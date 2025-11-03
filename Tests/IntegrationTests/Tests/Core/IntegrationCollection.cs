using IntegrationTests.Constants;
using IntegrationTests.WebApplicationFactories;

using Xunit;

namespace IntegrationTests.Tests.Core;

[CollectionDefinition(CollectionConstants.IntegrationCollection)]
public sealed class IntegrationCollection : ICollectionFixture<ApiWebApplicationFactory>
{
}
