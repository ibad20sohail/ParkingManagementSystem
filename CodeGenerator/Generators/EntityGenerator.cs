using Scriban;
using CodeGenerator.Models;
using CodeGenerator.Helpers;

namespace CodeGenerator.Generators;

public class EntityGenerator : BaseGenerator
{
    private readonly string _templatePath;


    public EntityGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }


    public async Task GenerateAsync(List<TableMetadata> tables, string outputFolder)
    {
        Console.WriteLine("--------Entity--------\n");

        Directory.CreateDirectory(outputFolder);

        var expectedFiles = tables
            .Select(t => $"{NamingHelper.ToPascalCase(t.Name)}.cs")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        var templateText = await File.ReadAllTextAsync(_templatePath);

        var template = Template.Parse(templateText);
        if (template.HasErrors)
        {
            throw new Exception(template.Messages.ToString());
        }

        foreach (var table in tables)
        {
            var properties = table.Columns.Select(c => new
            {
                name = NamingHelper.ToPascalCase(c.Name),
                type = SqlTypeMapper.Map(c.SqlType, c.IsNullable)
            });

            var entityName = NamingHelper.ToPascalCase(table.Name);
            
            var model = new
            {
                name = entityName,
                properties = properties
            };

            var result = await template.RenderAsync(model);

            var filePath = Path.Combine(outputFolder, $"{entityName}.cs");

            await WriteFileAsync(filePath, result);
        }

        Console.WriteLine("\n\n");
    }
}