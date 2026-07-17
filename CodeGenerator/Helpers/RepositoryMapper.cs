using CodeGenerator.Constants;
using CodeGenerator.Models;

namespace CodeGenerator.Helpers
{
    public static class RepositoryMapper
    {
        public static RepositoryMethodMetadata Map(ProcedureMetadata procedure)
        {
            var methodName = $"{procedure.Action}{procedure.Entity}{Cons.Async}";

            var request = procedure.Parameters.Count > 0
                ? $"{procedure.Action}{procedure.Entity}{Cons.Request}"
                : null;

            var response = procedure.IsOperation
                ? $"{Cons.Operation}{Cons.Response}"
                : $"{procedure.Action}{procedure.Entity}{Cons.Response}";

            return new RepositoryMethodMetadata
            {
                Name = methodName,
                RequestType = request,
                ResponseType = response,
                ProcedureName = procedure.Name
            };
        }
    }
}
