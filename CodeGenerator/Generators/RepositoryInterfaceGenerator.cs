using CodeGenerator.Models;

namespace CodeGenerator.Generators;

public class RepositoryInterfaceGenerator : RepositoryBaseGenerator
{
    private readonly string _templatePath;

    public RepositoryInterfaceGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }

    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder)
    {
        Console.WriteLine("--------Repository Interface--------\n");

        Directory.CreateDirectory(outputFolder);

        var repositories = BuildRepositories(procedures);

        var expectedFiles = repositories
            .Select(r => $"{r.InterfaceName}.cs")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        foreach (var repository in repositories)
        {
            var folder = Path.Combine(outputFolder, repository.EntityName);

            Directory.CreateDirectory(folder);

            var result = await RenderTemplateAsync(_templatePath, BuildModel(repository));

            await WriteFileAsync(Path.Combine(folder, $"{repository.InterfaceName}.cs"), result);
        }
        Console.WriteLine("\n\n");
    }
}
