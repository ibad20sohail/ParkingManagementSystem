namespace CodeGenerator.Helpers;

public static class SqlTypeMapper
{
    public static string Map(string sqlType, bool nullable)
    {
        string csharpType = sqlType.ToLower() switch
        {
            "int" => "int",

            "bigint" => "long",

            "smallint" => "short",

            "tinyint" => "byte",

            "bit" => "bool",

            "decimal" => "decimal",

            "numeric" => "decimal",

            "money" => "decimal",

            "float" => "double",

            "real" => "float",

            "datetime" => "DateTime",

            "datetime2" => "DateTime",

            "smalldatetime" => "DateTime",

            "date" => "DateTime",

            "time" => "TimeSpan",

            "uniqueidentifier" => "Guid",

            "varchar" => "string",

            "nvarchar" => "string",

            "char" => "string",

            "nchar" => "string",

            "text" => "string",

            "ntext" => "string",

            "xml" => "string",

            _ => "string"
        };


        if (nullable && csharpType != "string")
        {
            return csharpType + "?";
        }


        return csharpType;
    }
}