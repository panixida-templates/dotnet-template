using Bl.Implementations;
using Bl.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Bl.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseBl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISettingsBl, SettingsBl>();
        serviceCollection.AddScoped<IUsersBl, UsersBl>();

        return serviceCollection;
    }
}