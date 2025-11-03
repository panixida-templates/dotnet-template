using Api.Infrastructure.Models;

using Common.Constants.ApiEndpoints;
using Common.ConvertParams;
using Common.SearchParams;

using IntegrationTests.Tests.Core;
using IntegrationTests.WebApplicationFactories;

using Xunit;

using static IntegrationTests.Constants.TraitsConstants;

namespace IntegrationTests.Tests;

[Trait(TraitKeysConstants.Resource, UsersApiEndpointsConstants.ResourceNameConstant)]
public sealed class UsersControllerTests(ApiWebApplicationFactory apiWebApplicationFactory)
    : CrudTestsBase<UsersApiEndpointsConstants, int, UserModel, UsersSearchParams, UsersConvertParams>(apiWebApplicationFactory)
{
}
