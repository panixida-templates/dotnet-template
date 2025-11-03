using CodeGeneration.ServerCodeGenerator.Enums;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class NamingProvider
{
    internal static string GetPrefix(GeneratedFile layer) => layer switch
    {
        GeneratedFile.IDal => "I",
        GeneratedFile.IBl => "I",
        _ => string.Empty
    };

    internal static string GetNameWithSuffix(GeneratedFile layer, string Name) => layer switch
    {
        GeneratedFile.Entity => Name,
        GeneratedFile.SearchParams => $"{Inflection.Plural(Name)}SearchParams",
        GeneratedFile.ConvertParams => $"{Inflection.Plural(Name)}ConvertParams",
        GeneratedFile.DalFilters => $"{Inflection.Plural(Name)}Filters",
        GeneratedFile.DalIncludes => $"{Inflection.Plural(Name)}Includes",
        GeneratedFile.IDal => $"{Inflection.Plural(Name)}Dal",
        GeneratedFile.Dal => $"{Inflection.Plural(Name)}Dal",
        GeneratedFile.IBl => $"{Inflection.Plural(Name)}Bl",
        GeneratedFile.Bl => $"{Inflection.Plural(Name)}Bl",
        GeneratedFile.ApiController => $"{Inflection.Plural(Name)}Controller",
        GeneratedFile.ApiModel => $"{Name}Model",
        _ => string.Empty
    };
}
