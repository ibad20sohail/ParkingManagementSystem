using CodeGenerator.Constants;
using CodeGenerator.Models;

namespace CodeGenerator.Helpers;

public static class RepositoryMapper
{
    public static RepositoryMethodMetadata Map(
    ProcedureMetadata procedure)
    {
        var methodName =
            $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Async}";


        var requestType =
            procedure.Parameters.Count > 0
            ?
            $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Request}"
            :
            null;


        var responseType =
            procedure.IsOperation
            ?
            $"{Cons.Operation}{Cons.Response}"
            :
            $"{procedure.Action}{procedure.Entity}{procedure.Suffix}{Cons.Response}";


        var parameters = procedure.Parameters
            .Select(x => new RepositoryParameterMetadata
            {
                Name = x.Name,
                PropertyName =
                    NamingHelper.ToPascalCase(
                        x.Name.Replace("@", ""))
            })
            .ToList();


        return new RepositoryMethodMetadata
        {
            Name = methodName,
            RequestType = requestType,
            ResponseType = responseType,
            ProcedureName = procedure.Name,
            ReturnsCollection = procedure.ReturnsCollection,
            Parameters = parameters
        };
    }
}
