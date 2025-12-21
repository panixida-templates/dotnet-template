using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pl.Api.Host.Extensions.Configurations;

public static class GrpcClientsConfiguration
{
    public static void ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddGrpcClient<ApplicationUserService.ApplicationUserServiceClient>(options
        //    => options.Address = new Uri(configuration.GetValue<string>(AppsettingsKeysConstants.IdentityServiceBaseAddress)!));
    }
}
