using Common.Constants.ApiEndpoints;
using Common.ConvertParams;
using Common.SearchParams;

using IntegrationTests.Tests.Core;
using IntegrationTests.WebApplicationFactories;
using Pl.Api.Http.Dtos.Models;
using Xunit;

using static IntegrationTests.Constants.TraitsConstants;

namespace IntegrationTests.Tests;

[Trait(TraitKeysConstants.Resource, SettingsApiEndpointsConstants.ResourceNameConstant)]
public sealed class SettingsControllerTests(ApiWebApplicationFactory apiWebApplicationFactory)
    : CrudTestsBase<SettingsApiEndpointsConstants, int, SettingDto, SettingsSearchParams, SettingsConvertParams>(apiWebApplicationFactory)
{
}
