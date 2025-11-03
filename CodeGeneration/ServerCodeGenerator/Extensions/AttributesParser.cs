using System.Collections.Generic;
using System.Linq;

using Common.Attributes;
using Common.Enums;

using Microsoft.CodeAnalysis;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class AttributesParser
{
    internal record SearchDefinition(string BaseName, string Name, string Left, string Right, SearchParamType Kind);
    internal record FilterDefinition(string BaseName, string Name, string Left, string Right, FilterType Kind);

    internal static SearchDefinition? TryParseSearchParams(IPropertySymbol property)
    {
        var attribute = property.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(SearchParamAttribute));
        if (attribute is null)
        {
            return null;
        }

        string name = string.Empty, left = string.Empty, right = string.Empty;
        var kind = SearchParamType.Value;

        ParseArguments(attribute.ConstructorArguments, attribute.NamedArguments, ref name, ref left, ref right, ref kind);

        if (!string.IsNullOrEmpty(name)
            && !string.IsNullOrEmpty(left)
            && !string.IsNullOrEmpty(right)
            && kind.Equals(default(SearchParamType)))
        {
            kind = SearchParamType.Range;
        }

        ApplyDefault(property.Name, ref name, ref left, ref right);

        return new SearchDefinition(property.Name, name, left, right, kind);
    }

    internal static FilterDefinition? TryParseFilter(IPropertySymbol property)
    {
        var attribute = property.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(FilterAttribute));
        if (attribute is null) return null;

        string name = string.Empty, left = string.Empty, right = string.Empty;
        var kind = FilterType.AccurateComparison;

        ParseArguments(attribute.ConstructorArguments, attribute.NamedArguments, ref name, ref left, ref right, ref kind);
        ApplyDefault(property.Name, ref name, ref left, ref right);

        return new FilterDefinition(property.Name, name, left, right, kind);
    }

    private static void ParseArguments<TKind>(
        IEnumerable<TypedConstant> ctorArgs,
        IEnumerable<KeyValuePair<string, TypedConstant>> namedArgs,
        ref string name, ref string left, ref string right,
        ref TKind kind) where TKind : struct
    {
        foreach (var arg in ctorArgs)
        {
            if (arg.Type?.SpecialType == SpecialType.System_String)
            {
                if (string.IsNullOrEmpty(name)) name = (string)arg.Value!;
                else if (string.IsNullOrEmpty(left)) left = name;
                if (string.IsNullOrEmpty(left)) left = (string)arg.Value!;
                else right = (string)arg.Value!;
            }
            else if (arg.Type?.Name == typeof(TKind).Name)
            {
                kind = (TKind)arg.Value!;
            }
        }

        foreach (var (key, val) in namedArgs)
        {
            switch (key)
            {
                case nameof(SearchParamAttribute.ParameterName):
                    name = (string)val.Value!;
                    break;
                case nameof(SearchParamAttribute.LeftBorder):
                    left = (string)val.Value!;
                    break;
                case nameof(SearchParamAttribute.RightBorder):
                    right = (string)val.Value!;
                    break;
                case nameof(SearchParamAttribute.SearchParamType):
                case nameof(FilterAttribute.FilterType):
                    kind = (TKind)val.Value!;
                    break;
            }
        }
    }

    private static void ApplyDefault(string baseName, ref string name, ref string left, ref string right)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = baseName;
        }
        if (string.IsNullOrEmpty(left))
        {
            left = baseName + "From";
        }
        if (string.IsNullOrEmpty(right))
        {
            right = baseName + "To";
        }
    }
}
