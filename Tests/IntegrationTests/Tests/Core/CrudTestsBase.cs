using Common.Constants.ApiEndpoints.Core;
using Common.SearchParams.Core;

using FluentAssertions;

using IntegrationTests.WebApplicationFactories;
using Pl.Api.Http.Dtos.Models.Core;
using Xunit;

namespace IntegrationTests.Tests.Core;

public abstract class CrudTestsBase<TEndpoint, TId, TModel, TSearchParams, TConvertParams>(ApiWebApplicationFactory apiWebApplicationFactory)
    : IntegrationTestBase<TEndpoint, TId, TModel, TSearchParams, TConvertParams>(apiWebApplicationFactory)
    where TEndpoint : IBaseApiEndpointsConstants<TEndpoint, TId>
    where TId : notnull
    where TModel : BaseDto<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class
{
    [Fact(DisplayName = "GET by id → 200 и возвращает сущность, если она существует")]
    public async Task GetById_Should_Return200AndEntity_When_Exists()
    {
        await ResetDatabaseAsync();
        var id = await CreateAsync();
        await GetAsync(id);
    }

    [Fact(DisplayName = "GET by id → 404, если сущность не найдена")]
    public async Task GetById_Should_Return404_When_NotFound()
    {
        await ResetDatabaseAsync();
        var id = await CreateNotFoundIdAsync();
        await GetNotFoundAsync(id);
    }

    [Theory(DisplayName = "GET by filter → 200 и возвращает пагинированный результат")]
    [InlineData(5)]
    [InlineData(11)]
    [InlineData(8)]
    public async Task GetByFilter_Should_Return200AndPagedResult_When_DataExists(int count)
    {
        await ResetDatabaseAsync();

        for (int i = 0; i < count; i++)
        {
            await CreateAsync();
        }

        var result = await GetByFilterAsync();
        result.Total.Should().BeLessThanOrEqualTo(count, $"{HttpMethod.Get} {TEndpoint.GetByFilter()} не должен вернуть больше элементов, чем создано ({count})");
    }

    [Fact(DisplayName = "POST → 201 и положительный Id при валидной модели")]
    public async Task Create_Should_Return201AndPositiveId_When_ModelIsValid()
    {
        await ResetDatabaseAsync();
        await CreateAsync();
    }

    [Fact(DisplayName = "PUT → 200 и сохраняет изменения")]
    public async Task Update_Should_Return200_When_Exists()
    {
        await ResetDatabaseAsync();
        var id = await CreateAsync();
        var model = await GetAsync(id);
        await UpdateAsync(id, model);
        await GetAsync(id);
    }

    [Fact(DisplayName = "DELETE → 204 и последующий GET → 404")]
    public async Task Delete_Should_Return204AndSubsequentGet404_When_Deleted()
    {
        await ResetDatabaseAsync();
        var id = await CreateAsync();
        await DeleteAsync(id);
        await GetNotFoundAsync(id);
    }
}
