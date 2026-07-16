using CodeGenerator.Constants;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

namespace CodeGenerator.Generators;

public class ResponseGenerator : BaseGenerator
{
    private readonly string _responseTemplatePath;
    private readonly string _operationTemplatePath;


    public ResponseGenerator(string responseTemplatePath,string operationTemplatePath)
    {
        _responseTemplatePath = responseTemplatePath;
        _operationTemplatePath = operationTemplatePath;
    }


    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder)
    {
        Console.WriteLine("--------Response Model generation--------\n");

        Directory.CreateDirectory(outputFolder);

        var expectedFiles = procedures
           .Where(p => !p.IsOperation)
           .Where(p => p.ResultColumns.Count > 0)
           .Select(p => $"{p.Action}{p.Entity}{Cons.ResponseModel}.cs")
           .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        await GenerateOperationResponseAsync(outputFolder);

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
                Name = NamingHelper.ToPascalCase(c.Name),
                Type = SqlTypeMapper.Map(c.SqlType, c.IsNullable)
            });

            var model = new
            {
                Name = $"{procedure.Action}{procedure.Entity}{Cons.ResponseModel}",
                Properties = properties
            };

            var result = await template.RenderAsync(model);

            var filePath = Path.Combine(outputFolder, $"{model.Name}.cs");

            await WriteFileAsync(filePath, result);
        }

        Console.WriteLine("\n--------Response Model generation--------\n");
    }


    private async Task GenerateOperationResponseAsync(string outputFolder)
    {
        var filePath = Path.Combine(outputFolder, "OperationResponse.cs");

        if (File.Exists(filePath))
        {
            Console.WriteLine("Skipped: OperationResponse.cs");
            return;
        }

        var template = await File.ReadAllTextAsync(_operationTemplatePath);

        await WriteFileAsync(filePath, template);
    }
    private bool IsOperationProcedure(ProcedureMetadata procedure)
    {
        return procedure.Action == Cons.Add
            || procedure.Action == Cons.Edit
            || procedure.Action == Cons.Delete;
    }
}