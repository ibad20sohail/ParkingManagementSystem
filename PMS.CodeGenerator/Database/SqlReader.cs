using Microsoft.Data.SqlClient;
using PMS.CodeGenerator.Models;

namespace PMS.CodeGenerator.Database
{
    public class SqlReader
    {
        private readonly string _connectionString;

        public SqlReader(string connectionString)
        {
            _connectionString = connectionString;
        }

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
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}
