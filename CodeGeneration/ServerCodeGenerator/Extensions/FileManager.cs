using System;
using System.IO;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class FileManager
{
    internal static bool Exists(string path) => File.Exists(path);

    internal static void EnsureDirectory(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Путь к директории не может быть пустым", nameof(directoryPath));
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    internal static void WriteFile(string path, string content, bool overwrite = true)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            EnsureDirectory(directory);
        }

        if (!overwrite && Exists(path))
        {
            return;
        }

        File.WriteAllText(path, content);
    }

    internal static void DeleteFile(string path)
    {
        if (Exists(path))
        {
            File.Delete(path);
        }
    }
}
