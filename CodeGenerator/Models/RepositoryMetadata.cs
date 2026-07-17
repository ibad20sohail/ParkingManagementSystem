using CodeGenerator.Constants;

namespace CodeGenerator.Models
{
    public class RepositoryMetadata
    {
        public string EntityName { get; set; } = string.Empty;

        public string InterfaceName => $"I{EntityName}{Cons.Repository}";

        public string ClassName => $"{EntityName}{Cons.Repository}";

        public List<RepositoryMethodMetadata> Methods { get; set; } = new();
    }
}
