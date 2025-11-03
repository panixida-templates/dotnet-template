using Common.Enums;

using Dal.DbModels.Core;

namespace Dal.DbModels;

public sealed class Settings : BaseDbModel<int>
{
    public SettingType SettingType { get; set; }
    public string Value { get; set; } = string.Empty;
}
