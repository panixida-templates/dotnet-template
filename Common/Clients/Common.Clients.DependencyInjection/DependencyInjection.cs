using Common.Clients.Implementations;
using Common.Clients.Interfaces;
using Common.Constants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Net.Http.Headers;
using System.Net.Mime;

namespace Common.Clients.DependencyInjection;

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

        services.AddScoped<IApiHttpClient, ApiHttpClient>();

        return services;
    }
}
