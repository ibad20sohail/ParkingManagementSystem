namespace CodeGenerator.Models;

public class ResultColumnMetadata
{
    public string Name { get; set; } = string.Empty;

    public string SqlType { get; set; } = string.Empty;

    public bool IsNullable { get; set; }
}
