using CodeGenerator.Constants;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

namespace CodeGenerator.Generators;

public abstract class RepositoryBaseGenerator : BaseGenerator
{
    protected static List<RepositoryMetadata> BuildRepositories(List<ProcedureMetadata> procedures)
    {
        return procedures.GroupBy(x => x.Entity)
            .Select(group => new RepositoryMetadata
            {
                EntityName = group.Key,
                Methods = group
                    .Select(RepositoryMapper.Map)
                    .ToList()
            })
            .ToList();
    }

    protected object BuildModel(RepositoryMetadata repository)
    {
        var response = IsUsingResponseNamespaceValid(repository.EntityName, repository.Methods.Select(x => x.ResponseType).Distinct().ToList())
                ? $"using {Cons.ResponseGenerationPath.Replace('\\', '.')}.{repository.EntityName};"
                : string.Empty;
        return new
        {
            interface_name = repository.InterfaceName,
            class_name = repository.ClassName,
            file_prefix = Cons.FilePrefix,
            name_space_implementation = $"{Cons.RepositoryImplementationGenerationPath.Replace('\\', '.')}.{repository.EntityName}",
            name_space_interface = $"{Cons.RepositoryInterfaceGenerationPath.Replace('\\', '.')}.{repository.EntityName}",
            using_name_space_interface = $"using {Cons.RepositoryInterfaceGenerationPath.Replace('\\', '.')}.{repository.EntityName};",
            using_common = $"using {Cons.CommonResponseGenerationPath.Replace('\\', '.')};",
            using_request = $"using {Cons.RequestGenerationPath.Replace('\\', '.')}.{repository.EntityName};",
            using_response = response,
            using_dapper = $"using {Cons.UsingDapper};",
            using_sytem_data = $"using {Cons.UsingSystemData};",

            methods = repository.Methods.Select(m => new
            {
                name = m.Name,
                request_type = m.RequestType,
                response_type = m.ResponseType,
                procedure_name = m.ProcedureName,
                returns_collection = m.ReturnsCollection,
                parameters = m.Parameters.Select(p => new
                {
                    sql_name = p.Name,
                    property_name = p.PropertyName

                })
            })
        };
    }

    protected async Task<string> RenderTemplateAsync(string templatePath, object model)
    {
        var text = await File.ReadAllTextAsync(templatePath);

        var template = Template.Parse(text);

        if (template.HasErrors)
            throw new Exception(template.Messages.ToString());

        return await template.RenderAsync(model);
    }

    private bool IsUsingResponseNamespaceValid(string entityName, List<string> responseTypes)
    {
        responseTypes.Remove($"{Cons.Operation}{Cons.Response}");
        var solutionRoot = SolutionFinder.FindRoot();
        foreach (var r in responseTypes)
        {
            var path = Path.Combine(solutionRoot, Cons.ResponseGenerationPath, entityName, $"{r}.cs");

            if (File.Exists(path))
                return true;
        }

        return false;
    }
}