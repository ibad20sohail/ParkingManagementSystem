using CodeGenerator.Models;

namespace CodeGenerator.Helpers;

public static class ProcedureNameParser
{
    public static ProcedureNameMetadata Parse(string procedureName, IEnumerable<string> tableNames)
    {
        if (!procedureName.StartsWith("usp_", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception($"Invalid procedure name: {procedureName}");
        }

        var nameWithoutPrefix = procedureName.Substring(4);

        var index = nameWithoutPrefix.IndexOf('_');

        if (index == -1)
        {
            throw new Exception($"Invalid procedure name: {procedureName}");
        }

        // Example:
        // get_user_by_id
        // action = Get
        // remaining = user_by_id
        var action = NamingHelper.ToPascalCase(
            nameWithoutPrefix[..index]);

        var remaining = nameWithoutPrefix[(index + 1)..];


        // Find entity from database tables
        // Longest match first because:
        // parking_space_statuses
        // parking_spaces
        // parking_space
        // should match the most specific table
        var matchedTable = tableNames
            .OrderByDescending(x =>
                NamingHelper.NormalizeEntityName(x).Length)
            .FirstOrDefault(table =>
            {
                var normalizedTable =
                    NamingHelper.NormalizeEntityName(table);

                return remaining.StartsWith(
                    normalizedTable,
                    StringComparison.OrdinalIgnoreCase);
            });


        if (matchedTable is null)
        {
            throw new Exception(
                $"Unable to determine entity for procedure '{procedureName}'.");
        }


        // Keep entity name same as database entity
        // users -> Users
        // parking_spaces -> ParkingSpaces
        var entity = NamingHelper.ToPascalCase(NamingHelper.NormalizeEntityName(matchedTable));

        // Remove matched entity part to get operation suffix
        var normalizedEntity =
            NamingHelper.NormalizeEntityName(matchedTable);


        var remainingAfterEntity =
            remaining[normalizedEntity.Length..];


        // _by_id -> ById
        var suffixName = string.Concat(
            remainingAfterEntity
                .Trim('_')
                .Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Select(NamingHelper.ToPascalCase)
        );


        return new ProcedureNameMetadata
        {
            OriginalName = procedureName,
            Action = action,
            Entity = entity,
            Suffix = suffixName
        };
    }
}