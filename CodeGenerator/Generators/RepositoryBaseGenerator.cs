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
        string nameSpaceImplementation = string.Concat(
            Cons.RepositoryImplementationGenerationPath.Replace('\\', '.'), 
            ".",
            repository.EntityName);

        string nameSpaceInterface = string.Concat(
            Cons.RepositoryInterfaceGenerationPath.Replace('\\','.'),
            ".",
            repository.EntityName);

        return new
        {
            interface_name = repository.InterfaceName,
            class_name = repository.ClassName,
            file_prefix = Cons.FilePrefix,
            name_space_implementation = nameSpaceImplementation,
            name_space_interface = nameSpaceInterface,
            using_common = Cons.CommonResponseGenerationPath.Replace('\\', '.'),
            using_request = Cons.RequestGenerationPath.Replace('\\', '.'),
            using_response = Cons.ResponseGenerationPath.Replace('\\', '.'),
            using_dapper = Cons.UsingDapper,
            using_sytem_data = Cons.UsingSystemData,

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
}