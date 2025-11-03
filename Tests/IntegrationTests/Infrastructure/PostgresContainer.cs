using Common.Constants;

using Dal.Ef;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Npgsql;

using Respawn;
using Respawn.Graph;

using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure;

public sealed class PostgresContainer
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private Respawner? _respawner;

    public PostgresContainer()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString(AppsettingsKeysConstants.DefaultDbConnectionString));

        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase(npgsqlConnectionStringBuilder.Database)
            .WithUsername(npgsqlConnectionStringBuilder.Username)
            .WithPassword(npgsqlConnectionStringBuilder.Password)
            .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
            .WithCleanUp(true)
            .Build();
    }

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var options = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        using var defaultDbContext = new DefaultDbContext(options);
        await defaultDbContext.Database.MigrateAsync();

        await using var npgsqlConnection = new NpgsqlConnection(ConnectionString);
        await npgsqlConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(npgsqlConnection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = [new Table("__EFMigrationsHistory")],
        });
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
        {
            throw new InvalidOperationException("Respawner не инициализирован. Убедись, что InitializeAsync уже выполнен.");
        }

        await using var npgsqlConnection = new NpgsqlConnection(ConnectionString);
        await npgsqlConnection.OpenAsync();
        await _respawner.ResetAsync(npgsqlConnection);
    }
}