namespace CodeGenerator.Models;

public class ProcedureNameMetadata
{
    public string OriginalName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string ClassName
    {
        get { return $"{Action}{Entity}"; }
    }

    public string RequestName
    {
        get { return $"{ClassName}Request"; }
    }
}
