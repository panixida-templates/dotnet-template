using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Dal.Migrator.Extensions;

internal static class MigrationsApplier
{
    internal static async Task ApplyMigrationAsync(
        DbContext db,
        IReadOnlyList<MigrationOperation> difference,
        string migrationId,
        bool applyEntities,
        bool applyHistory)
    {
        ArgumentNullException.ThrowIfNull(difference);
        if (string.IsNullOrWhiteSpace(migrationId)) throw new ArgumentException("migrationId не может быть пустым", nameof(migrationId));

        using var transaction = await db.Database.BeginTransactionAsync();

        if (applyEntities)
        {
            await ApplyEntityChangesAsync(db, difference);
        }

        if (applyHistory)
        {
            await ApplyHistoryRowAsync(db, migrationId);
        }

        await transaction.CommitAsync();
    }

    private static async Task ApplyEntityChangesAsync(
        DbContext db,
        IReadOnlyList<MigrationOperation> difference)
    {
        var designTimeModel = db.GetService<IDesignTimeModel>()!.Model;

        var sqlGenerator = db.GetService<IMigrationsSqlGenerator>();
        var commands = sqlGenerator.Generate(difference, designTimeModel);

        foreach (var command in commands.Where(c => !string.IsNullOrWhiteSpace(c.CommandText)))
        {
            await db.Database.ExecuteSqlRawAsync(command.CommandText);
        }
    }

    private static async Task ApplyHistoryRowAsync(
        DbContext db,
        string migrationId)
    {
        var historyRepository = db.GetService<IHistoryRepository>();

        var createHistorySql = historyRepository.GetCreateIfNotExistsScript();
        await db.Database.ExecuteSqlRawAsync(createHistorySql);

        var efVersion = typeof(DbContext).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
            ?? "Unknown";

        var historyRow = new HistoryRow(migrationId, efVersion);
        var insertHistorySql = historyRepository.GetInsertScript(historyRow);
        await db.Database.ExecuteSqlRawAsync(insertHistorySql);
    }
}
