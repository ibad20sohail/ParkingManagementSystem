using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Models
{
    public class RepositoryMethodMetadata
    {
        public string Name { get; set; } = string.Empty;

        public string? RequestType { get; set; }

        public string ResponseType { get; set; } = string.Empty;

        public string ProcedureName { get; set; } = string.Empty;
        public bool ReturnsCollection { get; set; }
    }
}
