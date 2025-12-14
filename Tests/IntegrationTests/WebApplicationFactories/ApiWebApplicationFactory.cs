using Common.Clients.DependencyInjection;

using Common.Constants;

using IntegrationTests.Infrastructure;
using IntegrationTests.Mocks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace IntegrationTests.WebApplicationFactories;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgresContainer? _postgresContainer;

    async Task IAsyncLifetime.InitializeAsync()
    {
        _postgresContainer = new PostgresContainer();
        await _postgresContainer.InitializeAsync();
        CreateClient();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (_postgresContainer is not null)
        {
            await _postgresContainer.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentConstants.Test);

        builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
        {
            var testJson = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            configurationBuilder.AddJsonFile(testJson, optional: true, reloadOnChange: false);
            var appsettingsDictionary = new Dictionary<string, string?>
            {
                [AppsettingsKeysConstants.ConnectionStringsDefaultDbConnectionString] = _postgresContainer?.ConnectionString,
            };
            configurationBuilder.AddInMemoryCollection(appsettingsDictionary);
        });

        builder.ConfigureTestServices(services =>
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            if (_postgresContainer is not null)
            {
                services.AddSingleton(_postgresContainer);
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthenticationMock.SchemeName;
                options.DefaultChallengeScheme = AuthenticationMock.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, AuthenticationMock>(AuthenticationMock.SchemeName, _ => { });

            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(AuthenticationMock.SchemeName)
                .RequireAuthenticatedUser()
                .Build());

            services.UseHttpClients(configuration);
            services.AddHttpClient(ClientsConstants.ApiClient).ConfigurePrimaryHttpMessageHandler(_ => Server.CreateHandler());
        });
    }
}
