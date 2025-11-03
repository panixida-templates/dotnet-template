using System;
using System.Diagnostics;
using System.IO;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class MigrationsRunner
{
    internal static void BuildAndRun(string pathToMigrator)
    {
        var fullPathToMigrator = Path.GetFullPath(pathToMigrator);
        var workDirectory = Path.GetDirectoryName(fullPathToMigrator)!;

        Run("dotnet", $"build \"{fullPathToMigrator}\" -c Release", workDirectory);
        Run("dotnet", $"run --no-build --project \"{fullPathToMigrator}\" -c Release", workDirectory);
    }

    private static void Run(string fileName, string args, string? workingDir = null)
    {
        using var process = new Process
        {
            StartInfo = new()
            {
                FileName = fileName,
                Arguments = args,
                WorkingDirectory = workingDir ?? Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        process.StartInfo.EnvironmentVariables.Remove("MSBUILD_EXE_PATH");
        process.StartInfo.EnvironmentVariables.Remove("MSBUILDSDKSPATH");

        process.Start();

        Console.Write(process.StandardOutput.ReadToEnd());
        Console.Error.Write(process.StandardError.ReadToEnd());

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Команда «{fileName} {args}» завершилась с кодом {process.ExitCode}");
        }
    }
}
