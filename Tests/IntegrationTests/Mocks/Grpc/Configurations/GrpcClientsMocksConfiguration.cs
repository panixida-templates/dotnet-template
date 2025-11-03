using Gen.IdentityService.ApplicationUserService;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.Mocks.Grpc.Configurations;

internal static class GrpcClientsMocksConfiguration
{
    public static IServiceCollection AddGrpcMockss(this IServiceCollection services)
    {
        services.RemoveAll<ApplicationUserService.ApplicationUserServiceClient>();
        var applicationUserServiceMock = ApplicationUserServiceClientMock.CreateDefault();
        services.AddSingleton(applicationUserServiceMock);

        return services;
    }
}
