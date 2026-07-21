using CodeGenerator.Constants;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

namespace CodeGenerator.Generators;

public class RequestGenerator  : BaseGenerator
{
    private readonly string _templatePath;

    public RequestGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }

    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder)
    {
        Console.WriteLine("--------Request Model--------\n");

        Directory.CreateDirectory(outputFolder);

        var expectedFiles = procedures
            .Where(p => p.Parameters.Count > 0)
            .Select(p => $"{p.Action}{p.Entity}{Cons.Request}.cs")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        var templateText = await File.ReadAllTextAsync(_templatePath);

        var template = Template.Parse(templateText);
        if (template.HasErrors)
        {
            throw new Exception(
                template.Messages.ToString());
        }

        foreach (var procedure in procedures)
        {
            // only generate requests for procedures having parameters
            if (procedure.Parameters.Count == 0)
                continue;

            var properties = procedure.Parameters
                .Select(p => new
                {
                    Name = NamingHelper.ToPascalCase(p.Name),
                    Type = SqlTypeMapper.Map(p.SqlType, p.HasDefaultValue)
                });

            var model = new
            {
                Name = $"{procedure.Action}{procedure.Entity}{Cons.Request}",
                Properties = properties,
                file_prefix = Cons.FilePrefix
            };

            var result = await template.RenderAsync(model);

            var filePath = Path.Combine(outputFolder, $"{model.Name}.cs");

            await WriteFileAsync(filePath, result);
        }
        Console.WriteLine("\n\n");
    }
}