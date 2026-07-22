using CodeGenerator.Models;

namespace CodeGenerator.Generators;

public class RepositoryImplementationGenerator : RepositoryBaseGenerator
{
    private readonly string _templatePath;

    public RepositoryImplementationGenerator(string templatePath)
    {
        _templatePath = templatePath;
    }

    public async Task GenerateAsync(List<ProcedureMetadata> procedures, string outputFolder)
    {
        Console.WriteLine("--------Repository Implementation--------\n");

        Directory.CreateDirectory(outputFolder);

        var repositories = BuildRepositories(procedures);

        var expectedFiles = repositories
            .Select(r => $"{r.ClassName}.cs")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await DeleteOrphanFilesAsync(outputFolder, expectedFiles);

        foreach (var repository in repositories)
        {
            var folder = Path.Combine(outputFolder, repository.EntityName);

            Directory.CreateDirectory(folder);

            var result = await RenderTemplateAsync(_templatePath, BuildModel(repository));

            await WriteFileAsync(Path.Combine(folder, $"{repository.ClassName}.cs"), result);
        }

        Console.WriteLine("\n\n");
    }
}
