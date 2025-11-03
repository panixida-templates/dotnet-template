using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

using Api.Infrastructure.Responses.Core;

using Common.Constants;
using Common.SearchParams.Core;

using FluentAssertions;

using IntegrationTests.Clients.Interfaces;

namespace IntegrationTests.Clients.Implementations;

public sealed class ApiHttpClient : IApiHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    private static readonly HashSet<HttpMethod> MethodsWithBody =
    [
        HttpMethod.Post,
        HttpMethod.Put,
        HttpMethod.Patch
    ];

    public ApiHttpClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task<RestApiResponse<TResponse>?> GetAsync<TResponse>(
        string endpoint,
        BaseSearchParams? searchParams = null,
        object? convertParams = null,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Get, endpoint, searchParams, convertParams, headers, body: null);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    public Task<RestApiResponse<TResponse>?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.Created,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Post, endpoint, searchParams: null, convertParams: null, headers, body: request);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    public Task<RestApiResponse<TResponse>?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Put, endpoint, searchParams: null, convertParams: null, headers, body: request);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    public Task<RestApiResponse<TResponse>?> DeleteAsync<TResponse>(
        string endpoint,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.NoContent,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Delete, endpoint, searchParams: null, convertParams: null, headers, body: null);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    private async Task<RestApiResponse<T>?> SendAsync<T>(
        HttpRequestMessage request,
        HttpStatusCode expectedStatus,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(ClientsConstants.ApiClient);
        var url = request.RequestUri?.ToString() ?? string.Empty;

        using var response = await client.SendAsync(request, cancellationToken);
        var raw = response.Content is null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken);

        response.Should().NotBeNull($"HTTP {request.Method} {url} вернул null-ответ");
        response.StatusCode.Should().Be(expectedStatus, $"Ожидался статус {expectedStatus}, но получен {response.StatusCode}. Content: {raw}");

        return DeserializeResponse<T>(raw);
    }

    private static HttpRequestMessage BuildRequestMessage(
        HttpMethod method,
        string endpoint,
        BaseSearchParams? searchParams,
        object? convertParams,
        IDictionary<string, string?>? headers,
        object? body)
    {
        var url = BuildUrl(endpoint, searchParams, convertParams);
        var request = new HttpRequestMessage(method, url);

        if (headers is { Count: > 0 })
        {
            foreach (var (key, value) in headers)
            {
                if (!string.IsNullOrWhiteSpace(key) && value is not null)
                {
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }
        }

        if (body is not null && MethodsWithBody.Contains(method))
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        return request;
    }

    private static string BuildUrl(string endpoint, BaseSearchParams? searchParams, object? convertParams)
    {
        var stringBuilder = new StringBuilder(endpoint);
        var hasQuery = endpoint.Contains('?');

        if (searchParams is not null)
        {
            foreach (var (key, value) in KeyValueParametersToQuery(searchParams))
            {
                AppendQueryParameter(stringBuilder, ref hasQuery, key, value);
            }
        }
        if (convertParams is not null)
        {
            foreach (var (key, value) in KeyValueParametersToQuery(convertParams))
            {
                AppendQueryParameter(stringBuilder, ref hasQuery, key, value);
            }
        }

        return stringBuilder.ToString();
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

    private static IEnumerable<KeyValuePair<string, string?>> KeyValueParametersToQuery(object obj)
    {
        var type = obj.GetType();
        foreach (var propertyInfo in type.GetProperties())
        {
            var name = propertyInfo.Name;
            var value = propertyInfo.GetValue(obj);
            if (value is null)
            {
                continue;
            }

            if (value is System.Collections.IEnumerable sequence
                && value is not IEnumerable<byte>
                && value is not IEnumerable<char>
                && value is not string)
            {
                foreach (var item in sequence)
                {
                    var formatted = FormatForQuery(item);
                    if (formatted is not null)
                    {
                        yield return new(name, formatted);
                    }
                }
                continue;
            }

            var single = FormatForQuery(value);
            if (single is not null)
            {
                yield return new(name, single);
            }
        }
    }

    private static string? FormatForQuery(object? value)
    {
        return value switch
        {
            null => null,
            DateTime dateTime => DateTime.SpecifyKind(dateTime, dateTime.Kind == DateTimeKind.Unspecified ? DateTimeKind.Utc : dateTime.Kind).ToUniversalTime().ToString("o", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly timeOnly => timeOnly.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            Enum e => e.ToString(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    private static RestApiResponse<T>? DeserializeResponse<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<RestApiResponse<T>>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
