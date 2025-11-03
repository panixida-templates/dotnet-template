using System.Linq;

using Common.Attributes;

using Microsoft.CodeAnalysis;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class PropertiesExtractor
{
    internal static string GetIdType(INamedTypeSymbol symbol)
    {
        for (var current = symbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && current.TypeArguments.Length == 1)
            {
                return current.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
        }

        return "int";
    }

    internal static object GetPropertiesInfo(IPropertySymbol property)
    {
        var typeSymbol = property.Type;
        var typeName = typeSymbol
            .WithNullableAnnotation(property.NullableAnnotation)
            .ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var isNullable = typeName.EndsWith('?');

        if (property.NullableAnnotation == NullableAnnotation.Annotated && !isNullable)
        {
            typeName += "?";
        }

        var isNavigation = property.GetAttributes().Any(a => a.AttributeClass?.Name == nameof(NavigationAttribute));
        var isCollection = typeSymbol is IArrayTypeSymbol
            || (typeSymbol.SpecialType != SpecialType.System_String
            && typeSymbol.AllInterfaces.Any(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T))
            || (typeSymbol.SpecialType != SpecialType.System_String
            && typeSymbol.AllInterfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable));

        return new
        {
            name = property.Name,
            type = typeName,
            camel_name = char.ToLowerInvariant(property.Name[0]) + property.Name[1..],
            is_navigation = isNavigation,
            is_nullable = isNullable,
            is_collection = isCollection
        };
    }
}
