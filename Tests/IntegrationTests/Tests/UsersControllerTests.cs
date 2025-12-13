using Common.Constants.ApiEndpoints;
using Common.ConvertParams;
using Common.SearchParams;

using IntegrationTests.Tests.Core;
using IntegrationTests.WebApplicationFactories;
using Pl.Api.Http.Dtos.Models;
using Xunit;

using static IntegrationTests.Constants.TraitsConstants;

namespace IntegrationTests.Tests;

[Trait(TraitKeysConstants.Resource, UsersApiEndpointsConstants.ResourceNameConstant)]
public sealed class UsersControllerTests(ApiWebApplicationFactory apiWebApplicationFactory)
    : CrudTestsBase<UsersApiEndpointsConstants, int, UserDto, UsersSearchParams, UsersConvertParams>(apiWebApplicationFactory)
{
}
