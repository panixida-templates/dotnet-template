using Common.SearchParams.Core;

using System.Net;

namespace Common.Clients.Interfaces;

public interface IApiHttpClient
{
    Task<TResponse> GetAsync<TResponse>(
        string endpoint,
        BaseSearchParams? searchParams = null,
        object? convertParams = null,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default);

    Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.Created,
        CancellationToken cancellationToken = default);

    Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default);

    Task<TResponse> DeleteAsync<TResponse>(
        string endpoint,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.NoContent,
        CancellationToken cancellationToken = default);
}
