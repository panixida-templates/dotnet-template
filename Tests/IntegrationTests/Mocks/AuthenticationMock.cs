using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Mocks;

internal sealed class AuthenticationMock(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    internal const string HeaderApplicationUserId = "X-Test-ApplicationUserId";
    internal const string HeaderUserId = "X-Test-UserId";
    internal const string HeaderUser = "X-Test-User";
    internal const string HeaderRoles = "X-Test-Roles";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var applicationUserId = Request.Headers.TryGetValue(HeaderApplicationUserId, out var a) ? a.ToString() : "1";
        var userId = Request.Headers.TryGetValue(HeaderUserId, out var i) ? i.ToString() : "1";
        var userName = Request.Headers.TryGetValue(HeaderUser, out var u) ? u.ToString() : "it-user";

        var rolesRaw = Request.Headers.TryGetValue(HeaderRoles, out var r) ? r.ToString() : string.Empty;
        var roles = rolesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, applicationUserId),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        claims.Add(new Claim(ClaimTypes.Name, userName));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
