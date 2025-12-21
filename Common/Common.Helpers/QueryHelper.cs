using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;

namespace Common.Helpers;

public static class QueryHelper
{
    public static string BuildUrl<TSearchParams>(string endpoint, TSearchParams? searchParams = null)
        where TSearchParams : class
    {
        return BuildUrl<TSearchParams, object>(endpoint, searchParams, null);
    }


    public static string BuildUrl<TSearchParams, TConvertParams>(string endpoint, TSearchParams? searchParams = null, TConvertParams? convertParams = null)
        where TSearchParams : class
        where TConvertParams : class
    {
        var builder = new StringBuilder(endpoint);
        var hasQuery = endpoint.Contains('?');

        if (searchParams is not null)
        {
            foreach (var (key, value) in ToKeyValuePairs(searchParams))
            {
                AppendQueryParameter(builder, ref hasQuery, key, value);
            }
        }

        if (convertParams is not null)
        {
            foreach (var (key, value) in ToKeyValuePairs(convertParams))
            {
                AppendQueryParameter(builder, ref hasQuery, key, value);
            }
        }

        return builder.ToString();
    }

    public static IEnumerable<KeyValuePair<string, string?>> ToKeyValuePairs(object obj)
    {
        var type = obj.GetType();
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var name = propertyInfo.Name;
            var value = propertyInfo.GetValue(obj);

            if (value is null)
            {
                continue;
            }

            if (value is System.Collections.IEnumerable sequence &&
                value is not string &&
                value is not byte[] &&
                value is not char[])
            {
                foreach (var item in sequence)
                {
                    var formatted = FormatForQuery(item);
                    if (formatted != null)
                    {
                        yield return new KeyValuePair<string, string?>(name, formatted);
                    }
                }

                continue;
            }

            var single = FormatForQuery(value);
            if (single != null)
            {
                yield return new KeyValuePair<string, string?>(name, single);
            }
        }
    }

    public static string? FormatForQuery(object? value)
    {
        return value switch
        {
            null => null,
            bool b => b ? "true" : "false",
            DateTime dt => DateTime.SpecifyKind(dt, dt.Kind == DateTimeKind.Unspecified ? DateTimeKind.Utc : dt.Kind).ToUniversalTime().ToString("o", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly timeOnly => timeOnly.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            Enum e => e.ToString(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    private static void AppendQueryParameter(StringBuilder builder, ref bool hasQuery, string key, string? value)
    {
        builder.Append(hasQuery ? '&' : '?');
        hasQuery = true;

        builder
            .Append(HttpUtility.UrlEncode(key))
            .Append('=')
            .Append(HttpUtility.UrlEncode(value ?? string.Empty));
    }
}

