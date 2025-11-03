using System.Collections.Generic;
using System.Linq;

using Common.Enums;

using Microsoft.CodeAnalysis;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class SearchFilterGenerator
{
    private const string ItemPrefixConstant = "item";
    private const string DbQueryConstant = "dbObjects";

    internal static IEnumerable<string> GetSearchParams(INamedTypeSymbol model)
    {
        foreach (var property in model.GetMembers().OfType<IPropertySymbol>())
        {
            var defenition = AttributesParser.TryParseSearchParams(property);
            if (defenition is null)
            {
                continue;
            }

            var propertyType = property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace("?", "");

            if (defenition.Kind == SearchParamType.Value)
            {
                yield return $"public {propertyType}? {defenition.Name} {{ get; set; }}";
            }
            else
            {
                yield return $"public {propertyType}? {defenition.Left} {{ get; set; }}";
                yield return $"public {propertyType}? {defenition.Right} {{ get; set; }}";
            }
        }
    }

    internal static IEnumerable<string> GetFilters(INamedTypeSymbol model)
    {
        foreach (var property in model.GetMembers().OfType<IPropertySymbol>())
        {
            var defenition = AttributesParser.TryParseFilter(property);
            if (defenition is null)
            {
                continue;
            }

            var isString = property.Type.SpecialType == SpecialType.System_String;
            var isValueType = property.Type.IsValueType;

            var item = $"{ItemPrefixConstant}.{defenition.BaseName}";

            foreach (var line in GenerateFilterLines(defenition, isString, isValueType, item))
            {
                yield return line;
            }
        }
    }

    private static IEnumerable<string> GenerateFilterLines(dynamic defenition, bool isString, bool isValueType, string item)
    {
        return defenition.Kind switch
        {
            FilterType.AccurateComparison =>
                (IEnumerable<string>)(isString
                    ? BuildAccurateStringFilterBlock(defenition, isString, isValueType, item)
                    : BuildAccurateValueFilterBlock(defenition, isString, isValueType, item)),

            FilterType.InaccurateComparison =>
                (IEnumerable<string>)(isString
                    ? BuildContainsStringFilterBlock(defenition, isString, isValueType, item)
                    : BuildAccurateValueFilterBlock(defenition, isString, isValueType, item)),

            FilterType.InRange =>
                BuildRangeFilterBlock(defenition, isString, isValueType, item, ">=", "<="),

            FilterType.OutRange =>
                BuildOutOfRangeFilterBlock(defenition, isString, isValueType, item),

            FilterType.GreaterThan =>
                BuildRelationalFilterBlock(defenition, isString, isValueType, item, ">"),

            FilterType.GreaterThanOrEqual =>
                BuildRelationalFilterBlock(defenition, isString, isValueType, item, ">="),

            FilterType.LessThan =>
                BuildRelationalFilterBlock(defenition, isString, isValueType, item, "<"),

            FilterType.LessThanOrEqual =>
                BuildRelationalFilterBlock(defenition, isString, isValueType, item, "<="),

            _ => Enumerable.Empty<string>(),
        };
    }

    private static IEnumerable<string> BuildAccurateStringFilterBlock(dynamic defenition, bool isString, bool isValueType, string item)
    {
        var name = defenition.Name;

        var predicate = BuildEqualityExpression(
            $"{item}.ToLower()",
            $"{BuildSearchParamAccess(name)}.ToLower().Trim()");

        return BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(name)),
            BuildWhereClause(predicate));
    }

    private static IEnumerable<string> BuildContainsStringFilterBlock(dynamic defenition, bool isString, bool isValueType, string item)
    {
        var name = defenition.Name;

        var predicate = $"{item}.ToLower().Contains({BuildSearchParamAccess(name)}.ToLower().Trim())";

        return BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(name)),
            BuildWhereClause(predicate));
    }

    private static IEnumerable<string> BuildAccurateValueFilterBlock(dynamic defenition, bool isString, bool isValueType, string item)
    {
        var name = defenition.Name;

        var predicate = BuildEqualityExpression(
            item,
            BuildValueAccessExpression(isValueType, BuildSearchParamAccess(name)));

        return BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(name)),
            BuildWhereClause(predicate));
    }

    private static IEnumerable<string> BuildRangeFilterBlock(dynamic defenition, bool isString, bool isValueType, string item, string leftOperator, string rightOperator)
    {
        var leftName = defenition.Left;
        var rightName = defenition.Right;

        foreach (var line in BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(leftName)),
            BuildWhereClause(
                BuildBinaryOperationExpression(
                    item,
                    leftOperator,
                    BuildValueAccessExpression(isValueType, BuildSearchParamAccess(leftName))))))
            yield return line;

        foreach (var line in BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(rightName)),
            BuildWhereClause(
                BuildBinaryOperationExpression(
                    item,
                    rightOperator,
                    BuildValueAccessExpression(isValueType, BuildSearchParamAccess(rightName))))))
            yield return line;
    }

    private static IEnumerable<string> BuildOutOfRangeFilterBlock(dynamic defenition, bool isString, bool isValueType, string item)
    {
        var leftName = defenition.Left;
        var rightName = defenition.Right;

        var condition =
            $"{BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(leftName))} && " +
            $"{BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(rightName))}";

        var predicate =
            $"{item} < {BuildValueAccessExpression(isValueType, BuildSearchParamAccess(leftName))} || " +
            $"{item} > {BuildValueAccessExpression(isValueType, BuildSearchParamAccess(rightName))}";

        return BuildIfBlock(condition, BuildWhereClause(predicate));
    }

    private static IEnumerable<string> BuildRelationalFilterBlock(dynamic defenition, bool isString, bool isValueType, string item, string comparisonOperator)
    {
        var name = defenition.Name;

        var predicate = BuildBinaryOperationExpression(
            item,
            comparisonOperator,
            BuildValueAccessExpression(isValueType, BuildSearchParamAccess(name)));

        return BuildIfBlock(
            BuildNullCheckExpression(isString, isValueType, BuildSearchParamAccess(name)),
            BuildWhereClause(predicate));
    }

    private static string BuildSearchParamAccess(string name)
    {
        return $"searchParams.{name}";
    }

    private static string BuildEqualityExpression(string left, string right)
    {
        return $"{left} == {right}";
    }

    private static string BuildBinaryOperationExpression(string left, string operation, string right)
    {
        return $"{left} {operation} {right}";
    }

    private static string BuildWhereClause(string predicate)
    {
        return $"{DbQueryConstant} = {DbQueryConstant}.Where({ItemPrefixConstant} => {predicate});";
    }

    private static IEnumerable<string> BuildIfBlock(string condition, params string[] body)
    {
        yield return $"if ({condition})";
        yield return "{";
        foreach (var line in body)
        {
            yield return "    " + line;
        }
        yield return "}";
    }

    private static string BuildValueAccessExpression(bool isValueType, string name)
    {
        return isValueType ? $"{name}.Value" : name;
    }

    private static string BuildNullCheckExpression(bool isString, bool isValueType, string name)
    {
        if (isString)
        {
            return $"!string.IsNullOrEmpty({name})";
        }
        if (isValueType)
        {
            return $"{name}.HasValue";
        }
        return $"{name} != null";
    }
}
