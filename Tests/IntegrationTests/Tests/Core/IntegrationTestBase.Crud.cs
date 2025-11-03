using System.Net;

using Api.Infrastructure.Core;

using AutoFixture;

using Common.SearchParams.Core;

using FluentAssertions;

namespace IntegrationTests.Tests.Core;

public abstract partial class IntegrationTestBase<TEndpoint, TId, TModel, TSearchParams, TConvertParams>
{
    protected async Task<TModel> GetAsync(TId id, TConvertParams? convertParams = null, IDictionary<string, string?>? headers = null)
    {
        var endpoint = TEndpoint.ById(id);
        var response = await ApiHttpClient.GetAsync<TModel>(endpoint: endpoint, convertParams: convertParams, headers: headers ?? DefaultHeaders);

        response.Should().NotBeNull($"{HttpMethod.Get} {endpoint} вернул null");
        response.IsSuccess.Should().BeTrue($"{HttpMethod.Get} {endpoint} должен завершиться успешно");
        response.Payload.Should().NotBeNull($"{HttpMethod.Get} {endpoint} не вернул payload");
        response.Payload.Id.Should().Be(id, $"{HttpMethod.Get} {endpoint} должен вернуть объект с тем же Id");

        return response.Payload!;
    }

    protected async Task GetNotFoundAsync(TId id, IDictionary<string, string?>? headers = null)
    {
        var endpoint = TEndpoint.ById(id);
        var response = await ApiHttpClient.GetAsync<TModel>(endpoint: endpoint, headers: headers ?? DefaultHeaders, expectedStatus: HttpStatusCode.NotFound);

        (response == null || !response.IsSuccess).Should().BeTrue($"{HttpMethod.Get} {endpoint} должен вернуть 404, если объект не найден");
    }

    protected async Task<SearchResult<TModel>> GetByFilterAsync(TSearchParams? searchParams = null, TConvertParams? convertParams = null, IDictionary<string, string?>? headers = null)
    {
        searchParams ??= TestDataFacade.Build<TSearchParams>().Without(item => item.SortField).Create();
        var endpoint = TEndpoint.GetByFilter();
        var response = await ApiHttpClient.GetAsync<SearchResult<TModel>>(endpoint: endpoint, searchParams: searchParams, convertParams: convertParams, headers: headers ?? DefaultHeaders);

        response.Should().NotBeNull($"{HttpMethod.Get} {endpoint} вернул null");
        response.IsSuccess.Should().BeTrue($"{HttpMethod.Get} {endpoint} должен завершиться успешно");
        response.Payload.Should().NotBeNull($"{HttpMethod.Get} {endpoint} не вернул payload");
        response.Payload.Objects.Should().NotBeNull($"{HttpMethod.Get} {endpoint} должен вернуть список объектов");
        response.Payload.RequestedPage.Should().Be(searchParams.Page, $"{HttpMethod.Get} {endpoint} должен вернуть корректную страницу");
        response.Payload.RequestedObjectsCount.Should().Be(searchParams.ObjectsCount, $"{HttpMethod.Get} {endpoint} должен вернуть корректный размер страницы");
        response.Payload.Objects.Count.Should().BeLessThanOrEqualTo(searchParams.ObjectsCount ?? int.MaxValue, $"{HttpMethod.Get} {endpoint} не должен вернуть больше объектов, чем запрошено");

        return response.Payload;
    }

    protected async Task<TId> CreateAsync(TModel? request = null, IDictionary<string, string?>? headers = null)
    {
        request ??= TestDataFacade.Create<TModel>();
        var endpoint = TEndpoint.Base();
        var response = await ApiHttpClient.PostAsync<TModel, TId>(endpoint: endpoint, request: request, headers: headers ?? DefaultHeaders);

        response.Should().NotBeNull($"{HttpMethod.Post} {endpoint} вернул null");
        response.IsSuccess.Should().BeTrue($"{HttpMethod.Post} {endpoint} должен завершиться успешно");
        response.Payload.Should().NotBeNull($"{HttpMethod.Post} {endpoint} не вернул payload");
        (response.Payload as int?).Should().BePositive($"{HttpMethod.Post} {endpoint} должен вернуть положительный идентификатор");

        return response.Payload;
    }

    protected async Task UpdateAsync(TId id, TModel request, IDictionary<string, string?>? headers = null)
    {
        request = TestDataFacade.Mutate(request);
        var endpoint = TEndpoint.ById(id);
        var response = await ApiHttpClient.PutAsync<TModel, NoContent>(endpoint: endpoint, request: request, headers: headers ?? DefaultHeaders);

        response.Should().NotBeNull($"{HttpMethod.Put} {endpoint} вернул null");
        response.IsSuccess.Should().BeTrue($"{HttpMethod.Put} {endpoint} должен завершиться успешно");
    }

    protected async Task DeleteAsync(TId id, IDictionary<string, string?>? headers = null)
    {
        var endpoint = TEndpoint.ById(id);
        var response = await ApiHttpClient.DeleteAsync<object>(endpoint: endpoint, headers: headers ?? DefaultHeaders);

        response.Should().BeNull($"{HttpMethod.Delete} {endpoint} должен вернуть пустой ответ (204 No Content)");
    }
}
