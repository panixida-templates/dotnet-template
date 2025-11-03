using System.Collections.Generic;
using System.Linq;

using Api.Infrastructure.Models.Core;

using Common.Enums;

namespace Api.Infrastructure.Models;

public sealed record SettingsModel : BaseModel<int>
{
    public required SettingType SettingType { get; set; }
    public required string Value { get; set; }

    public static SettingsModel FromEntity(Entities.Settings obj)
    {
        return new SettingsModel
        {
            Id = obj.Id,
            SettingType = obj.SettingType,
            Value = obj.Value,
        };
    }

    public static Entities.Settings ToEntity(SettingsModel obj)
    {
        return new Entities.Settings(
            id: obj.Id,
            settingType: obj.SettingType,
            value: obj.Value);
    }

    public static List<SettingsModel> FromEntitiesList(IEnumerable<Entities.Settings> list)
    {
        return list.Select(FromEntity).ToList()!;
    }

    public static List<Entities.Settings> ToEntitiesList(IEnumerable<SettingsModel> list)
    {
        return list.Select(ToEntity).ToList()!;
    }
}
