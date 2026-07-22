using CodeGenerator.Models;

namespace CodeGenerator.Helpers;

public static class ProcedureNameParser
{
    public static ProcedureNameMetadata Parse(string procedureName)
    {
        var parts = procedureName.Split('_', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
        {
            throw new Exception(
                $"Invalid procedure name format: {procedureName}");
        }


        var action = NamingHelper.ToPascalCase(parts[1]);


        var entityParts = parts
            .Skip(2)
            .Select(NamingHelper.ToPascalCase);


        var entity = string.Concat(entityParts);


        return new ProcedureNameMetadata
        {
            OriginalName = procedureName,
            Action = action,
            Entity = entity
        };
    }
}
