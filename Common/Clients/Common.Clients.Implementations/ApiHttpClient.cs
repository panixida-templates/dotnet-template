using Common.Clients.Interfaces;
using Common.Constants;
using Common.Helpers;
using Common.JsonConverters;
using Common.SearchParams.Core;

using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Clients.Implementations;

public sealed class ApiHttpClient : IApiHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter(),
            new UtcDateTimeConverter(),
            new NullableUtcDateTimeConverter(),
        }
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

    public Task<TResponse> GetAsync<TResponse>(
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

    public Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.Created,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Post, endpoint, searchParams: null, convertParams: null, headers, body: request);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    public Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Put, endpoint, searchParams: null, convertParams: null, headers, body: request);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    public Task<TResponse> DeleteAsync<TResponse>(
        string endpoint,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.NoContent,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = BuildRequestMessage(HttpMethod.Delete, endpoint, searchParams: null, convertParams: null, headers, body: null);
        return SendAsync<TResponse>(requestMessage, expectedStatus, cancellationToken);
    }

    private static HttpRequestMessage BuildRequestMessage(
        HttpMethod method,
        string endpoint,
        BaseSearchParams? searchParams,
        object? convertParams,
        IDictionary<string, string?>? headers,
        object? body)
    {
        var url = QueryHelper.BuildUrl(endpoint, searchParams, convertParams);
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

    private async Task<T> SendAsync<T>(
        HttpRequestMessage request,
        HttpStatusCode expectedStatus,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(ClientsConstants.ApiClient);
        var url = request.RequestUri?.ToString() ?? string.Empty;

        using var response = await client.SendAsync(request, cancellationToken);
        var raw = response.Content is null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken);

        ValidateStatusCode(request, expectedStatus, response.StatusCode, url, raw);

        if (ShouldReturnNoContent(expectedStatus, raw))
        {
            return default!;
        }

        return DeserializeOrThrow<T>(request, response.StatusCode, url, raw);
    }

    private static void ValidateStatusCode(
        HttpRequestMessage request,
        HttpStatusCode expectedStatus,
        HttpStatusCode actualStatus,
        string url,
        string raw)
    {
        if (actualStatus == expectedStatus)
        {
            return;
        }

        throw new HttpRequestException(
            message: $"Ожидался статус {expectedStatus}, но получен {actualStatus}. {request.Method} {url}. Content: {raw}",
            inner: null,
            statusCode: actualStatus);
    }

    private static bool ShouldReturnNoContent(HttpStatusCode expectedStatus, string raw)
    {
        if (expectedStatus == HttpStatusCode.NoContent)
        {
            return true;
        }

        return string.IsNullOrWhiteSpace(raw);
    }

    private static T DeserializeOrThrow<T>(
        HttpRequestMessage request,
        HttpStatusCode statusCode,
        string url,
        string raw)
    {
        try
        {
            var value = JsonSerializer.Deserialize<T>(raw, JsonOptions);

            return value is null
                ? throw new HttpRequestException(
                    message: $"Получен null при десериализации. {request.Method} {url}. Content: {raw}",
                    inner: null,
                    statusCode: statusCode)
                : value;
        }
        catch (JsonException)
        {
            throw new HttpRequestException(
                message: $"Ошибка десериализации. {request.Method} {url}. Content: {raw}",
                inner: null,
                statusCode: statusCode);
        }
    }
}
