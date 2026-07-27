using CodeGenerator.Constants;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

namespace CodeGenerator.Generators;

public class ResponseGenerator : BaseGenerator
{
    private readonly string _responseTemplatePath;
    private readonly string _operationResponseTemplatePath;
    private readonly string _appResponseTemplatePath;


    public ResponseGenerator(string responseTemplatePath,string operationResponseTemplatePath, string appResponseTemplatePath)
    {
        _responseTemplatePath = responseTemplatePath;
        _operationResponseTemplatePath = operationResponseTemplatePath;
        _appResponseTemplatePath = appResponseTemplatePath;
    }


    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder, string commonOutputFolder)
    {
        Console.WriteLine("--------Response Model--------\n");

        Directory.CreateDirectory(outputFolder);

        var expectedFiles = procedures
           .Where(p => !p.IsOperation)
           .Where(p => p.ResultColumns.Count > 0)
           .Select(p => $"{p.Action}{p.Entity}{p.Suffix}{Cons.Response}.cs")
           .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        await GenerateOperationResponseAsync(commonOutputFolder);

        await GenerateAppResponseAsync(commonOutputFolder);

        var templateText = await File.ReadAllTextAsync(_responseTemplatePath);

        var template = Template.Parse(templateText);

        foreach (var procedure in procedures)
        {
            // Add/Edit/Delete use OperationResponse
            if (procedure.IsOperation)
                continue;

            // only generate response classes when procedure returns data
            if (procedure.ResultColumns.Count == 0)
                continue;

            var properties = procedure.ResultColumns.Select(c => new
            {
                name = NamingHelper.ToPascalCase(c.Name),
                type = SqlTypeMapper.Map(c.SqlType, c.IsNullable)
            });

            var model = new
            {
                name = $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Response}",
                properties = properties,
                file_prefix = Cons.FilePrefix,
                name_space = string.Concat(Cons.ResponseGenerationPath.Replace('\\','.'), ".", procedure.Entity)
            };

            var result = await template.RenderAsync(model);
            
            var entityFolder = Path.Combine(outputFolder, procedure.Entity);

            Directory.CreateDirectory(entityFolder);

            var filePath = Path.Combine(entityFolder, $"{model.name}.cs");

            await WriteFileAsync(filePath, result);
        }
        Console.WriteLine("\n\n");

    }

    private async Task GenerateOperationResponseAsync(string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        var filePath = Path.Combine(outputFolder, $"{Cons.Operation}{Cons.Response}.cs");

        //if (File.Exists(filePath))
        //{
        //    Console.WriteLine($"Skipped: {Cons.Operation}{Cons.Response}.cs");
        //    return;
        //}

        var model = new
        {
            file_prefix = Cons.FilePrefix,
            name_space = Cons.CommonResponseGenerationPath.Replace('\\', '.')
        };
        var templateText = await File.ReadAllTextAsync(_operationResponseTemplatePath);
        var template = Template.Parse(templateText);

        var result = await template.RenderAsync(model);

        await WriteFileAsync(filePath, result);
    }

    private async Task GenerateAppResponseAsync(string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        var filePath = Path.Combine(outputFolder, $"{Cons.App}{Cons.Response}.cs");

        //if (File.Exists(filePath))
        //{
        //    Console.WriteLine($"Skipped: {Cons.App}{Cons.Response}.cs");
        //    return;
        //}

        var model = new
        {
            file_prefix = Cons.FilePrefix,
            name_space = Cons.CommonResponseGenerationPath.Replace('\\', '.')
        };
        var templateText = await File.ReadAllTextAsync(_appResponseTemplatePath);
        var template = Template.Parse(templateText);

        var result = await template.RenderAsync(model);

        await WriteFileAsync(filePath, result);
    }
}