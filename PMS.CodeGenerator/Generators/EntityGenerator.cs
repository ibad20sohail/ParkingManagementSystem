using Scriban;
using PMS.CodeGenerator.Models;
using PMS.CodeGenerator.Helpers;

namespace PMS.CodeGenerator.Generators;

public class EntityGenerator
{
    private readonly string _templatePath;


    public EntityGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }


    public async Task GenerateAsync(List<TableMetadata> tables, string outputFolder)
    {
        var templateText = await File.ReadAllTextAsync(_templatePath);

        var template =
            Template.Parse(templateText);

        foreach (var table in tables)
        {
            var properties = table.Columns.Select(c => new
            {
                name = c.Name,
                type = SqlTypeMapper.Map(c.SqlType, c.IsNullable)
            });

            var model = new
            {
                name = table.Name,
                properties
            };

            var result = await template.RenderAsync(model);

            Directory.CreateDirectory(outputFolder);

            var filePath = Path.Combine(outputFolder,$"{table.Name}.cs");

            await File.WriteAllTextAsync(filePath, result);
        }
    }
}