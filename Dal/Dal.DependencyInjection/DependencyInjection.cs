using Common.Constants;

using Dal.Ef;
using Dal.Implementations;
using Dal.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dal.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(AppsettingsKeysConstants.DefaultDbConnectionString)).UseSnakeCaseNamingConvention());

        serviceCollection.AddScoped<ISettingsDal, SettingsDal>();
        serviceCollection.AddScoped<IUsersDal, UsersDal>();

        return serviceCollection;
    }
}