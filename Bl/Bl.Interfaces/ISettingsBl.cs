using Bl.Interfaces.Core;

using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using Entities;

namespace Bl.Interfaces;

public interface ISettingsBl : ICrudBl<Settings, int, SettingsSearchParams, SettingsConvertParams>
{
    Task<Settings?> GetAsync(SettingType settingType);
}

