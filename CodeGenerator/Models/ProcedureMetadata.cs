using CodeGenerator.Constants;
using CodeGenerator.Enums;

namespace CodeGenerator.Models;

public class ProcedureMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public GeneratorResponseType ResponseType { get; set; } = GeneratorResponseType.Auto;
    //public bool UsesOperationResponse => ResultColumns.Count == 1 && ResultColumns[0].Name.Equals("message", StringComparison.OrdinalIgnoreCase);
    public bool ReturnsCollection { get; set; }

    public List<ParameterMetadata> Parameters { get; set; } = new();
    public List<ResultColumnMetadata> ResultColumns { get; set; } = new();
}
