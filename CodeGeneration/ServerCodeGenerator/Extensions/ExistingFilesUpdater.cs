using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CodeGeneration.ServerCodeGenerator.Enums;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RoslynFormatter = Microsoft.CodeAnalysis.Formatting.Formatter;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class ExistingFilesUpdater
{
    private const string DependencyInjectionFileName = "DependencyInjection";
    private const string DefaultDbContextFileName = "DefaultDbContext";

    internal static void AddDIRegistration(
        INamedTypeSymbol model,
        string templatePath,
        string directory,
        GeneratedFile layer)
    {
        var filePath = Path.Combine(directory, $"{DependencyInjectionFileName}.cs");
        EnsureFile(directory, filePath, () => TemplatesRenderer.Render(templatePath, model));

        var root = ParseSyntaxRoot(filePath);
        var suffix = layer == GeneratedFile.DalDependencyInjection ? "Dal" : "Bl";
        var implementationName = $"{Inflection.Plural(model.Name)}{suffix}";
        var interfaceName = $"I{implementationName}";
        var registrationName = $"AddScoped<{interfaceName}, {implementationName}>";

        if (root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(invocation => invocation.ToFullString().Contains(registrationName)))
        {
            Console.WriteLine($"В {DependencyInjectionFileName}: {registrationName} уже есть, пропуск");
            return;
        }

        var methodDeclaration = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .First(m => m.Identifier.Text == $"Use{suffix}");

        var statements = methodDeclaration.Body!.Statements;
        var idx = statements
            .Select((s, i) => (s, i))
            .First(pair => pair.s is ReturnStatementSyntax)
            .i;

        var indent = statements
            .First()
            .GetLeadingTrivia()
            .Where(t => t.IsKind(SyntaxKind.WhitespaceTrivia));
        var endOfLine = SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(Environment.NewLine));

        var newStatement = SyntaxFactory.ParseStatement($"serviceCollection.{registrationName}();")
            .WithLeadingTrivia(indent)
            .WithTrailingTrivia(endOfLine);
        var newBody = methodDeclaration.Body.WithStatements(
            idx >= 0
            ? statements.Insert(idx, newStatement)
            : statements.Add(newStatement));

        var newMethodDeclaration = methodDeclaration.WithBody(newBody);
        var newRoot = root.ReplaceNode(methodDeclaration, newMethodDeclaration);

        FileManager.WriteFile(filePath, newRoot.ToFullString());
        Console.WriteLine($"В {suffix}.{DependencyInjectionFileName}: добавлена {registrationName}");
    }

    internal static void AddDbSetInDbContext(
        INamedTypeSymbol model,
        string templatePath,
        string directory)
    {
        var filePath = Path.Combine(directory, $"{DefaultDbContextFileName}.cs");
        EnsureFile(directory, filePath, () => TemplatesRenderer.Render(templatePath, model));

        var root = ParseSyntaxRoot(filePath);

        var propertyName = Inflection.Plural(model.Name);

        if (root.DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Any(property => property.Identifier.Text == propertyName))
        {
            Console.WriteLine($"В {DefaultDbContextFileName}: DbSet<{model.Name}> уже есть, пропуск");
            return;
        }

        var classDeclaration = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(c => c.Identifier.Text == "DefaultDbContext");
        var propertyDeclaration = SyntaxFactory.ParseMemberDeclaration(
            $"public virtual DbSet<{model.Name}> {propertyName} {{ get; set; }}")!;

        var newClass = classDeclaration.AddMembers(propertyDeclaration);
        var newRoot = root.ReplaceNode(classDeclaration, newClass);

        var workspace = new AdhocWorkspace();
        var annotated = newRoot.WithAdditionalAnnotations(RoslynFormatter.Annotation);
        var formatted = RoslynFormatter.Format(annotated, workspace);

        FileManager.WriteFile(filePath, formatted.ToFullString());
        Console.WriteLine($"В {DefaultDbContextFileName}: добавлено DbSet<{model.Name}>");
    }

    internal static void RemoveDIRegistration(
        INamedTypeSymbol model,
        string directory,
        GeneratedFile layer)
    {
        var filePath = Path.Combine(directory, $"{DependencyInjectionFileName}.cs");
        if (!FileManager.Exists(filePath))
        {
            Console.WriteLine($"[RemoveDIRegistration] Файл не найден: {filePath}. Регистрация для {model.Name} не удалена.");
            return;
        }

        var lines = File.ReadAllLines(filePath);
        var suffix = layer == GeneratedFile.DalDependencyInjection ? "Dal" : "Bl";
        var implementationName = $"{Inflection.Plural(model.Name)}{suffix}";
        var interfaceName = $"I{implementationName}";
        var pattern = $"^\\s*serviceCollection\\.AddScoped<\\s*{Regex.Escape(interfaceName)}\\s*,\\s*{Regex.Escape(implementationName)}>.*$";

        var filtered = lines.Where(line => !Regex.IsMatch(line, pattern));

        FileManager.WriteFile(filePath, string.Join(Environment.NewLine, filtered));
        Console.WriteLine($"Из {DependencyInjectionFileName}: строка для {model.Name} удалена");
    }

    internal static void RemoveDbSetFromDbContext(
        INamedTypeSymbol model,
        string directory)
    {
        var filePath = Path.Combine(directory, $"{DefaultDbContextFileName}.cs");
        if (!FileManager.Exists(filePath))
        {
            Console.WriteLine($"[RemoveDbSetFromDbContext] Файл не найден: {filePath}. DbSet<{model.Name}> не удалён.");
            return;
        }

        var lines = File.ReadAllLines(filePath);
        var pattern = $"^\\s*public\\s+virtual\\s+DbSet<\\s*{Regex.Escape(model.Name)}\\s*>\\s+{Regex.Escape(Inflection.Plural(model.Name))}.*$";

        var filtered = lines.Where(line => !Regex.IsMatch(line, pattern));

        FileManager.WriteFile(filePath, string.Join(Environment.NewLine, filtered));
        Console.WriteLine($"Из {DefaultDbContextFileName}: DbSet<{model.Name}> удалён");
    }

    private static void EnsureFile(string directory, string path, Func<string> generate)
    {
        FileManager.EnsureDirectory(directory);
        if (!FileManager.Exists(path))
        {
            FileManager.WriteFile(path, generate());
        }
    }

    private static SyntaxNode ParseSyntaxRoot(string path)
    {
        var text = File.ReadAllText(path);
        return CSharpSyntaxTree.ParseText(text).GetRoot();
    }
}
