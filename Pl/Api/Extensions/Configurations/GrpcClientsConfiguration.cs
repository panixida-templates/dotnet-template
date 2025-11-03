using System;

using Common.Constants;

using Gen.IdentityService.ApplicationUserService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.Configurations;

public static class GrpcClientsConfiguration
{
    public static void ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<ApplicationUserService.ApplicationUserServiceClient>(options
            => options.Address = new Uri(configuration.GetValue<string>(AppsettingsKeysConstants.IdentityServiceBaseAddress)!));
    }
}
