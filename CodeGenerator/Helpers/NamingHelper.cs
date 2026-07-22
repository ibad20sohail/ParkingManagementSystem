using Humanizer;

namespace CodeGenerator.Helpers;

public class NamingHelper
{
    public static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return string.Concat(
            value.Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => char.ToUpperInvariant(x[0]) + x[1..].ToLowerInvariant())
            );
    }

    public static bool IsPlural(string word)
    {
        return word != word.Singularize();
    }
}
