using static IntegrationTests.Mocks.AuthenticationMock;

namespace IntegrationTests.DataFactories;

internal static class TestHeadersFactory
{
    public static IDictionary<string, string?> CreateAuthHeadersForUser(
        int applicationUserId = 1,
        int userId = 1,
        string userName = "test",
        IEnumerable<string>? roles = null)
    {
        return new Dictionary<string, string?>()
        {
            [HeaderApplicationUserId] = applicationUserId.ToString(),
            [HeaderUserId] = userId.ToString(),
            [HeaderUser] = userName,
            [HeaderRoles] = roles is null ? "" : string.Join(',', roles)
        };
    }
}
