using System.ComponentModel.DataAnnotations;

namespace Common.Helpers;

public static class EnumHelper
{
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
