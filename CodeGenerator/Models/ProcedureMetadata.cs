namespace CodeGenerator.Models
{
    public class ProcedureMetadata
    {
        public string Name { get; set; } = string.Empty;

        public string Operation { get; set; } = string.Empty;

        public string Entity { get; set; } = string.Empty;

        public List<ParameterMetadata> Parameters { get; set; } = new();

        public List<ResultColumnMetadata> ResultColumns { get; set; } = new();
    }
}
