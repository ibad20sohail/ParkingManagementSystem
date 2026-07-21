using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeGenerator.Database
{
    public class SqlReader
    {
        private readonly string _connectionString;

        public SqlReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Entity readers
        public async Task<List<string>> GetTablesAsync()
        {
            var tables = new List<string>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE='BASE TABLE'
                ORDER BY TABLE_NAME";

            using var cmd = new SqlCommand(sql, connection);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }

            return tables;
        }
        public async Task<List<ColumnMetadata>> GetColumnsAsync(string tableName)
        {
            var columns = new List<ColumnMetadata>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                CHARACTER_MAXIMUM_LENGTH
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME=@Table
                ORDER BY ORDINAL_POSITION";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Table", tableName);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                columns.Add(new ColumnMetadata
                {
                    Name = reader.GetString(0),
                    SqlType = reader.GetString(1),
                    IsNullable = reader.GetString(2) == "YES",
                    MaxLength = reader.IsDBNull(3)
                        ? null
                        : reader.GetInt32(3)
                });
            }

            return columns;
        }
        public async Task<List<string>> GetPrimaryKeysAsync(string tableName)
        {
            var primaryKeys = new List<string>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                WHERE OBJECTPROPERTY(
                    OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)),
                    'IsPrimaryKey'
                ) = 1
                AND TABLE_NAME = @Table";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Table", tableName);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                primaryKeys.Add(reader.GetString(0));
            }

            return primaryKeys;
        }
        public async Task<List<ForeignKeyMetadata>> GetForeignKeysAsync()
        {
            var foreignKeys = new List<ForeignKeyMetadata>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"

                SELECT
                fk.name AS ForeignKeyName,
                tp.name AS ParentTable,
                cp.name AS ParentColumn,
                tr.name AS ReferencedTable,
                cr.name AS ReferencedColumn

                FROM sys.foreign_keys fk

                INNER JOIN sys.foreign_key_columns fkc
                ON fk.object_id = fkc.constraint_object_id

                INNER JOIN sys.tables tp
                ON fkc.parent_object_id = tp.object_id

                INNER JOIN sys.columns cp
                ON fkc.parent_object_id = cp.object_id
                AND fkc.parent_column_id = cp.column_id

                INNER JOIN sys.tables tr
                ON fkc.referenced_object_id = tr.object_id

                INNER JOIN sys.columns cr
                ON fkc.referenced_object_id = cr.object_id
                AND fkc.referenced_column_id = cr.column_id
                ";


            using var cmd = new SqlCommand(sql, connection);

            using var reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                foreignKeys.Add(new ForeignKeyMetadata
                {
                    ForeignKeyName = reader.GetString(0),
                    ParentTable = reader.GetString(1),
                    ParentColumn = reader.GetString(2),
                    ReferencedTable = reader.GetString(3),
                    ReferencedColumn = reader.GetString(4)
                });
            }


            return foreignKeys;
        }
        public async Task<List<TableMetadata>> GetTableMetadataAsync()
        {
            var tablesMetadata = new List<TableMetadata>();

            var tables = await GetTablesAsync();
            var foreignKeys = await GetForeignKeysAsync();
            foreach (var tableName in tables)
            {
                var columns = await GetColumnsAsync(tableName);

                var primaryKeys = await GetPrimaryKeysAsync(tableName);


                foreach (var column in columns)
                {
                    if (primaryKeys.Contains(column.Name))
                    {
                        column.IsPrimaryKey = true;
                    }
                }


                tablesMetadata.Add(new TableMetadata
                {
                    Name = tableName,
                    Columns = columns,
                    ForeignKeys = foreignKeys.Where(x => x.ParentTable == tableName).ToList()
                });
            }


            return tablesMetadata;
        }
        #endregion

        #region SP readers
        public async Task<List<string>> GetStoredProceduresAsync()
        {
            var procedures = new List<string>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT ROUTINE_NAME
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_TYPE = 'PROCEDURE'
                ORDER BY ROUTINE_NAME";

            using var cmd = new SqlCommand(sql, connection);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                procedures.Add(reader.GetString(0));
            }

            return procedures;
        }
        public async Task<List<ParameterMetadata>> GetProcedureParametersAsync(string procedureName)
        {
            var parameters = new List<ParameterMetadata>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT
                    p.name AS ParameterName,
                    t.name AS DataType,
                    p.has_default_value,
                    p.is_output
                FROM sys.parameters p
                INNER JOIN sys.types t
                    ON p.user_type_id = t.user_type_id
                INNER JOIN sys.objects o
                    ON p.object_id = o.object_id
                WHERE o.name = @Procedure
                ORDER BY p.parameter_id";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Procedure", procedureName);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                parameters.Add(new ParameterMetadata
                {
                    Name = reader.GetString(0).Replace("@", ""),
                    SqlType = reader.GetString(1),
                    HasDefaultValue = reader.GetBoolean(2),
                    IsOutput = reader.GetBoolean(3)
                });
            }

            return parameters;
        }
        public async Task<List<ProcedureMetadata>> GetProceduresMetadataAsync()
        {
            var procedures = await GetStoredProceduresAsync();

            var result = new List<ProcedureMetadata>();


            foreach (var procedure in procedures)
            {
                var parsed = ProcedureNameParser.Parse(procedure);

                var parameters = await GetProcedureParametersAsync(procedure);
                var resultColumns = await GetProcedureResultColumnsAsync(procedure);
                result.Add(new ProcedureMetadata
                {
                    Name = procedure,
                    Action = parsed.Action,
                    Entity = parsed.Entity,
                    ReturnsCollection = parsed.Action.Equals("Get", StringComparison.OrdinalIgnoreCase) && NamingHelper.IsPlural(parsed.Entity),
                    Parameters = parameters,
                    ResultColumns = resultColumns
                });
            }

            return result;
        }
        public async Task<List<ResultColumnMetadata>> GetProcedureResultColumnsAsync(string procedureName)
        {
            var columns = new List<ResultColumnMetadata>();

            using var connection = CreateConnection();

            await connection.OpenAsync();

            var sql = @"
                SELECT
                    name,
                    system_type_name,
                    is_nullable
                FROM sys.dm_exec_describe_first_result_set_for_object
                (
                    OBJECT_ID(@Procedure),
                    NULL
                )
                WHERE is_hidden = 0
                ORDER BY column_ordinal";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Procedure", procedureName);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                columns.Add(new ResultColumnMetadata
                {
                    Name = reader.GetString(0),
                    SqlType = reader.GetString(1).Split('(')[0],
                    IsNullable = reader.GetBoolean(2)
                });
            }

            return columns;
        }
        #endregion 

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}
