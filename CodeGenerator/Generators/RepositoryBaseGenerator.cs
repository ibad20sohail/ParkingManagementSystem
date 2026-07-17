using CodeGenerator.Generators;
using CodeGenerator.Helpers;
using CodeGenerator.Models;
using Scriban;

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
        return new
        {
            interface_name = repository.InterfaceName,
            class_name = repository.ClassName,
            folder_name = repository.EntityName,

            methods = repository.Methods.Select(m => new
            {
                name = m.Name,
                request_type = m.RequestType,
                response_type = m.ResponseType,
                procedure_name = m.ProcedureName
            })
        };
    }

    protected async Task<string> RenderTemplateAsync(string templatePath,object model)
    {
        var text = await File.ReadAllTextAsync(templatePath);

        var template = Template.Parse(text);

        if (template.HasErrors)
            throw new Exception(template.Messages.ToString());

        return await template.RenderAsync(model);
    }
}