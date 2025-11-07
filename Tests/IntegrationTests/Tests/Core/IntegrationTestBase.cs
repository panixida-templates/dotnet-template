using Api.Infrastructure.Models.Core;

using Common.Constants.ApiEndpoints.Core;
using Common.SearchParams.Core;

using DataGenerator;

using Gen.IdentityService.Enums;

using IntegrationTests.Clients.Interfaces;
using IntegrationTests.Constants;
using IntegrationTests.DataFactories;
using IntegrationTests.Infrastructure;
using IntegrationTests.WebApplicationFactories;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

using static IntegrationTests.Constants.TraitsConstants;

namespace IntegrationTests.Tests.Core;

[Collection(CollectionConstants.IntegrationCollection)]
[Trait(TraitKeysConstants.Category, TraitCategoriesConstants.Integration)]
public abstract partial class IntegrationTestBase<TEndpoint, TId, TModel, TSearchParams, TConvertParams>
    where TEndpoint : IBaseApiEndpointsConstants<TEndpoint, TId>
    where TId : notnull
    where TModel : BaseModel<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class
{
    protected readonly IDictionary<string, string?> DefaultHeaders;
    protected readonly IApiHttpClient ApiHttpClient;

    private readonly PostgresContainer _postgresContainer;

    protected DataFacade TestDataFacade;

    protected IntegrationTestBase(ApiWebApplicationFactory apiWebApplicationFactory)
    {
        DefaultHeaders = TestHeadersFactory.CreateAuthHeadersForUser(roles: [nameof(ApplicationUserRole.Admin)]);
        ApiHttpClient = apiWebApplicationFactory.Services.GetRequiredService<IApiHttpClient>();

        _postgresContainer = apiWebApplicationFactory.Services.GetRequiredService<PostgresContainer>();

        TestDataFacade = new DataFacade(scope: GetType().FullName);
    }

    protected Task ResetDatabaseAsync()
    {
        return _postgresContainer.ResetDatabaseAsync();
    }

    protected async Task<TId> CreateNotFoundIdAsync()
    {
        var id = await CreateAsync();
        await DeleteAsync(id);
        return id;
    }
}
