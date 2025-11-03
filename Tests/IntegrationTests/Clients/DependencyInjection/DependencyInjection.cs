using System.Net.Http.Headers;
using System.Net.Mime;

using Common.Constants;

using IntegrationTests.Clients.Implementations;
using IntegrationTests.Clients.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Clients.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBase = configuration[AppsettingsKeysConstants.ApiBaseAddress]
              ?? throw new InvalidOperationException($"Config key '{AppsettingsKeysConstants.ApiBaseAddress}' is missing.");

        services.AddHttpClient(ClientsConstants.ApiClient, client =>
        {
            client.BaseAddress = new Uri(apiBase);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        });

        services.AddSingleton<IApiHttpClient, ApiHttpClient>();

        return services;
    }
}
