using System;
using System.IO;
using System.Linq;

using Common.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class ModelLoader
{
    public static INamedTypeSymbol Load(string pathToModel, string className)
    {
        var files = Directory.GetFiles(pathToModel, "*.cs", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            throw new InvalidOperationException($"В {pathToModel} нет .cs-файлов");
        }

        var trees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file));

        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && File.Exists(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();
        refs.Add(MetadataReference.CreateFromFile(typeof(SearchParamAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "Temp",
            trees,
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var symbol = compilation
            .GetSymbolsWithName(name => name.Equals(className, StringComparison.Ordinal), SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .FirstOrDefault();

        return symbol ?? throw new InvalidOperationException($"Класс {className} не найден");
    }
}
