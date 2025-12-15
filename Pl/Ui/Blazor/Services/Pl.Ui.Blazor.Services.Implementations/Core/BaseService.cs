using Common.Clients.Interfaces;
using Common.Constants.ApiEndpoints.Core;
using Common.SearchParams.Core;

using Pl.Api.Http.Dtos.Core;
using Pl.Ui.Blazor.Services.Interfaces.Core;

namespace Pl.Ui.Blazor.Services.Implementations.Core;

public abstract class BaseService<TEndpoint, TId, TDto, TViewModel, TSearchParams, TConvertParams>(IApiHttpClient apiHttpClient, Func<TDto, TViewModel> toViewModel, Func<TViewModel, TDto> toDto) 
    : IBaseService<TId, TViewModel, TSearchParams, TConvertParams>
    where TEndpoint : IBaseApiEndpointsConstants<TEndpoint, TId>
    where TId : notnull
    where TDto : class
    where TViewModel : class
    where TSearchParams : BaseSearchParams
    where TConvertParams : class
{
    public async Task<TViewModel> GetAsync(TId id, TConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var dto = (await apiHttpClient.GetAsync<RestApiResponse<TDto>>(
            endpoint: TEndpoint.ById(id),
            convertParams: convertParams,
            cancellationToken: cancellationToken)).Payload;

        return toViewModel(dto!);
    }

    public async Task<SearchResult<TViewModel>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var searchResult = (await apiHttpClient.GetAsync<RestApiResponse<SearchResult<TDto>>>(
            endpoint: TEndpoint.GetByFilter(),
            searchParams: searchParams,
            convertParams: convertParams,
            cancellationToken: cancellationToken)).Payload;

        return searchResult!.Map(toViewModel);
    }

    public async Task<TId> CreateAsync(TViewModel viewModel, CancellationToken cancellationToken = default)
    {
        return (await apiHttpClient.PostAsync<TDto, RestApiResponse<TId>>(
            endpoint: TEndpoint.Base(),
            request: toDto(viewModel),
            cancellationToken: cancellationToken)).Payload!;
    }

    public Task UpdateAsync(TId id, TViewModel viewModel, CancellationToken cancellationToken = default)
    {
        return apiHttpClient.PutAsync<TDto, RestApiResponse<NoContent>>(
            endpoint: TEndpoint.ById(id),
            request: toDto(viewModel),
            cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        return apiHttpClient.DeleteAsync<RestApiResponse<object>>(
            endpoint: TEndpoint.ById(id),
            cancellationToken: cancellationToken);
    }
}
