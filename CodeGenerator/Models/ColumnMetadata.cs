namespace CodeGenerator.Models;

public class ColumnMetadata
{
    public string Name { get; set; } = string.Empty;

    public string SqlType { get; set; } = string.Empty;

    public bool IsNullable { get; set; }

    public int? MaxLength { get; set; }

    public bool IsPrimaryKey { get; set; }
}
