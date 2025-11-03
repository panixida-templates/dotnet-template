using System;
using System.IO;

using CodeGeneration.ServerCodeGenerator.Configs;
using CodeGeneration.ServerCodeGenerator.Enums;
using CodeGeneration.ServerCodeGenerator.Extensions;

using Microsoft.CodeAnalysis;

namespace CodeGeneration.ServerCodeGenerator.Core;

internal static class CodeGenerator
{
    internal static void Run(JobConfig jobConfig)
    {
        foreach (var modelName in jobConfig.Models)
        {
            var model = ModelLoader.Load(jobConfig.PathToModels, modelName);
            ProcessModel(model, jobConfig);
        }

        if (jobConfig.NeedMigrate)
        {
            MigrationsRunner.BuildAndRun(jobConfig.PathToMigrator);
        }
    }

    private static void ProcessModel(INamedTypeSymbol model, JobConfig jobConfig)
    {
        foreach (var layer in jobConfig.GeneratedFiles)
        {
            if (!jobConfig.DeleteGenerated)
            {
                Generate(model, jobConfig, layer);
            }
            else
            {
                RemoveGenerated(model, jobConfig, layer);
            }
        }

        if (jobConfig.DeleteSourceModel)
        {
            var modelFile = Path.Combine(jobConfig.PathToModels, model.Name + ".cs");
            if (FileManager.Exists(modelFile))
            {
                FileManager.DeleteFile(modelFile);
                Console.WriteLine($"Исходная модель удалена: {modelFile}");
            }
        }
    }

    private static void Generate(INamedTypeSymbol model, JobConfig jobConfig, GeneratedFile layer)
    {
        if (!jobConfig.Templates.TryGetValue(layer, out var templatePath))
        {
            throw new InvalidOperationException($"Нет шаблона для {layer}");
        }

        if (layer == GeneratedFile.DalDependencyInjection || layer == GeneratedFile.BlDependencyInjection)
        {
            ExistingFilesUpdater.AddDIRegistration(model, templatePath, jobConfig.OutputDirectory[layer], layer);
            return;
        }
        if (layer == GeneratedFile.DefaultDbContext)
        {
            ExistingFilesUpdater.AddDbSetInDbContext(model, templatePath, jobConfig.OutputDirectory[layer]);
            return;
        }

        var code = TemplatesRenderer.Render(templatePath, model);
        var fileName = $"{NamingProvider.GetPrefix(layer)}{NamingProvider.GetNameWithSuffix(layer, model.Name)}.cs";
        var directory = jobConfig.OutputDirectory[layer];
        FileManager.EnsureDirectory(directory);
        var path = Path.Combine(directory, fileName);

        var existed = FileManager.Exists(path);
        FileManager.WriteFile(path, code, overwrite: jobConfig.OverwriteExisting);

        var status = GetStatus(existed, jobConfig.OverwriteExisting);
        Console.WriteLine($"{fileName} {status}");
    }

    private static void RemoveGenerated(INamedTypeSymbol model, JobConfig jobConfig, GeneratedFile layer)
    {
        var directory = jobConfig.OutputDirectory[layer];

        if (layer == GeneratedFile.DalDependencyInjection || layer == GeneratedFile.BlDependencyInjection)
        {
            ExistingFilesUpdater.RemoveDIRegistration(model, directory, layer);
            return;
        }
        if (layer == GeneratedFile.DefaultDbContext)
        {
            ExistingFilesUpdater.RemoveDbSetFromDbContext(model, directory);
            return;
        }

        var fileName = $"{NamingProvider.GetPrefix(layer)}{NamingProvider.GetNameWithSuffix(layer, model.Name)}.cs";
        var path = Path.Combine(directory, fileName);
        if (FileManager.Exists(path))
        {
            FileManager.DeleteFile(path);
            Console.WriteLine($"Удалён: {path}");
        }
    }

    private static string GetStatus(bool existed, bool overwrite)
    {
        if (!existed)
        {
            return "создан";
        }
        return overwrite ? "перезаписан" : "пропущен";
    }
}
