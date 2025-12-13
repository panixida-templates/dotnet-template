using System.Net;

using Common.SearchParams.Core;
using Pl.Api.Http.Dtos.Core;

namespace IntegrationTests.Clients.Interfaces;

public interface IApiHttpClient
{
    Task<RestApiResponse<TResponse>?> GetAsync<TResponse>(
        string endpoint,
        BaseSearchParams? searchParams = null,
        object? convertParams = null,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default);

    Task<RestApiResponse<TResponse>?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.Created,
        CancellationToken cancellationToken = default);

    Task<RestApiResponse<TResponse>?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        CancellationToken cancellationToken = default);

    Task<RestApiResponse<TResponse>?> DeleteAsync<TResponse>(
        string endpoint,
        IDictionary<string, string?>? headers = null,
        HttpStatusCode expectedStatus = HttpStatusCode.NoContent,
        CancellationToken cancellationToken = default);
}
