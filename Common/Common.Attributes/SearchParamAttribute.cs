using Common.Enums;

namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SearchParamAttribute : Attribute
{
    public string ParameterName { get; set; } = string.Empty;
    public string LeftBorder { get; set; } = string.Empty;
    public string RightBorder { get; set; } = string.Empty;
    public SearchParamType SearchParamType { get; set; } = default;

    public SearchParamAttribute() { }

    public SearchParamAttribute(string parameterName)
    {
        ParameterName = parameterName;
        SearchParamType = SearchParamType.Value;
    }

    public SearchParamAttribute(string leftBorder, string rightBorder)
    {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        SearchParamType = SearchParamType.Range;
    }

    public SearchParamAttribute(SearchParamType searchParamType)
    {
        SearchParamType = searchParamType;
    }
}
