using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RedisClient.Implementations;
using RedisClient.Interfaces;

using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

using StackExchange.Redis;

namespace RedisClient;

public static class DependencyInjection
{
    public static IServiceCollection UseRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string is missing.");

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });
        services.AddSingleton<IRedisCache>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return new RedisCache(multiplexer);
        });
        services.AddSingleton<IDistributedLockFactory>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            var multiplexers = new List<RedLockMultiplexer>
            {
                new RedLockMultiplexer(multiplexer)
            };
            return RedLockFactory.Create(multiplexers);
        });

        return services;
    }
}
