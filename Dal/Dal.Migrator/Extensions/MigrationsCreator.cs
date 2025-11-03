using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace Dal.Migrator.Extensions;

internal static class MigrationsCreator
{
    internal static ScaffoldedMigration CreateAndSaveMigration(
        IServiceProvider designServiceProvider,
        IReadOnlyList<MigrationOperation> difference,
        string contextNamespace,
        string projectPath)
    {
        var scaffolder = designServiceProvider.GetRequiredService<IMigrationsScaffolder>();

        var migrationName = MigrationsNameBuilder.BuildMigrationName(difference);
        var scaffolded = scaffolder.ScaffoldMigration(migrationName, contextNamespace);

        var outputDir = Path.Combine(projectPath, "Migrations");
        scaffolder.Save(projectPath, scaffolded, outputDir);

        return scaffolded;
    }
}
