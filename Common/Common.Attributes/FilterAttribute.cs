using Common.Enums;

namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FilterAttribute : Attribute
{
    public string ParameterName { get; set; } = string.Empty;
    public string LeftBorder { get; set; } = string.Empty;
    public string RightBorder { get; set; } = string.Empty;
    public FilterType FilterType { get; set; } = default;

    public FilterAttribute() { }

    public FilterAttribute(FilterType filterType)
    {
        FilterType = filterType;
    }

    public FilterAttribute(string parameterName, FilterType filterType)
    {
        ParameterName = parameterName;
        FilterType = filterType;
    }

    public FilterAttribute(string leftBorder, string rightBorder, FilterType filterType)
    {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
        FilterType = filterType;
    }
}
