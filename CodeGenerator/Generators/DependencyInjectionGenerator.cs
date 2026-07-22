using CodeGenerator.Constants;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

namespace CodeGenerator.Generators;

public class DependencyInjectionGenerator : BaseGenerator
{
    private readonly string _templatePath;

    public DependencyInjectionGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }

    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder)
    {
        Console.WriteLine("--------Dependency Injection--------\n");

        Directory.CreateDirectory(outputFolder);

        var repositories = procedures
            .GroupBy(x => x.Entity)
            .Select(g => new RepositoryMetadata
            {
                EntityName = g.Key,
                Methods = g.Select(RepositoryMapper.Map).ToList()
            })
            .ToList();

        var interfaceUsings = repositories
            .Select(x => $"{Cons.RepositoryInterfaceGenerationPath.Replace('\\', '.')}.{x.EntityName}")
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var implementationUsings = repositories
            .Select(x => $"{Cons.RepositoryImplementationGenerationPath.Replace('\\', '.')}.{x.EntityName}")
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var model = new
        {
            repositories = repositories.OrderBy(x => x.EntityName)
            .Select(x => new
            {
                interface_name = x.InterfaceName,
                class_name = x.ClassName
            }),
            interface_usings = interfaceUsings,
            implementation_usings = implementationUsings,
            using_dependency_injection = Cons.UsingDependencyInjection,
            name_space = Cons.DependencyInjectionGenerationPath.Replace('\\', '.'),
            file_prefix = Cons.FilePrefix,
            class_name = Cons.RepositoryServiceInjection
        };

        var templateText = await File.ReadAllTextAsync(_templatePath);

        var template = Template.Parse(templateText);

        if (template.HasErrors)
            throw new Exception(template.Messages.ToString());

        var result = await template.RenderAsync(model);

        await WriteFileAsync(Path.Combine(outputFolder, $"{Cons.RepositoryServiceInjection}.cs"), result);

        Console.WriteLine("\n\n");
    }
}