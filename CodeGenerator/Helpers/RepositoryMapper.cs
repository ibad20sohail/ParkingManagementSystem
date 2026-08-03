using CodeGenerator.Constants;
using CodeGenerator.Enums;
using CodeGenerator.Models;

namespace CodeGenerator.Helpers;

public static class RepositoryMapper
{
    public static RepositoryMethodMetadata Map(ProcedureMetadata procedure)
    {
        var methodName = $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Async}";

        var requestType = procedure.Parameters.Count > 0
                ? $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Request}"
                : null;

        var responseType = procedure.ResponseType switch
        {
            GeneratorResponseType.OperationResponse => $"{Cons.Operation}{Cons.Response}",
            GeneratorResponseType.None => $"{Cons.Void}",
            _ => $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Response}"
        };
        var parameters = procedure.Parameters
            .Select(x => new RepositoryParameterMetadata
            {
                Name = x.Name,
                PropertyName = NamingHelper.ToPascalCase(x.Name.Replace("@", ""))
            })
            .ToList();

        bool hasResponse = procedure.ResponseType != GeneratorResponseType.None;

        return new RepositoryMethodMetadata
        {
            Name = methodName,
            RequestType = requestType,
            ResponseType = responseType,
            ProcedureName = procedure.Name,
            ReturnsCollection = procedure.ReturnsCollection,
            Parameters = parameters,
            HasResponse = hasResponse
        };
    }
}