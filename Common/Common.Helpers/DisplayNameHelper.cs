using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Common.Helpers;

public static class DisplayNameHelper
{
    public static string DisplayNameFor(this Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
            ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{type.Name}'");

        var display = property.GetCustomAttribute<DisplayAttribute>();

        return display?.GetName() ?? property.Name;
    }

    public static string GetDisplayName<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        var displayAttribute = field
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .OfType<DisplayAttribute>()
            .FirstOrDefault();

        return displayAttribute?.GetName() ?? value.ToString();
    }
}
