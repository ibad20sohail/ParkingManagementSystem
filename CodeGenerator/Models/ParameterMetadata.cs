namespace CodeGenerator.Models
{
    public class ParameterMetadata
    {
        public string Name { get; set; } = string.Empty;

        public string SqlType { get; set; } = string.Empty;

        public bool HasDefaultValue { get; set; }

        public bool IsOutput { get; set; }
    }
}
