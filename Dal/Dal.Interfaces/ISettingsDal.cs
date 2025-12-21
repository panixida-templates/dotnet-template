using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using Dal.Interfaces.Core;

namespace Dal.Interfaces;

public interface ISettingsDal : IBaseDal<DbModels.Settings, Entities.Setting, int, SettingsSearchParams, SettingsConvertParams>
{
    Task<Entities.Setting?> GetAsync(SettingType settingType);
}
