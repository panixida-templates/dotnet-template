using Dal.DependencyInjection;
using Dal.Ef;
using Dal.Migrator.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.json")
       .AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.UseDal(ctx.Configuration);
    })
    .RunMigrationsAsync<DefaultDbContext>();
