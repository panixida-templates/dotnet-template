using Pluralize.NET.Core;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

public static class Inflection
{
    private static readonly Pluralizer _pluralizer = new();

    public static string Plural(string word) => _pluralizer.Pluralize(word);
}
