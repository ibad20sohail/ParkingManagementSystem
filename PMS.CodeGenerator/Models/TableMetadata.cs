namespace PMS.CodeGenerator.Models
{
    public class TableMetadata
    {
        public string Name { get; set; }
        public List<ColumnMetadata> Columns { get; set; }
        public List<ForeignKeyMetadata> ForeignKeys { get; set; } = new();
    }
}
