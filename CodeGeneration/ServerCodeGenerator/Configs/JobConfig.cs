using System.Collections.Generic;

using CodeGeneration.ServerCodeGenerator.Enums;

namespace CodeGeneration.ServerCodeGenerator.Configs;

internal sealed class JobConfig
{
    public string PathToModels { get; set; } = string.Empty;
    public string PathToMigrator { get; set; } = string.Empty;
    public List<string> Models { get; set; } = [];
    public List<GeneratedFile> GeneratedFiles { get; set; } = [];
    public Dictionary<GeneratedFile, string> Templates { get; set; } = [];
    public Dictionary<GeneratedFile, string> OutputDirectory { get; set; } = [];
    public bool OverwriteExisting { get; set; }
    public bool DeleteGenerated { get; set; }
    public bool DeleteSourceModel { get; set; }
    public bool NeedMigrate { get; set; }
}
