using Bl.Interfaces.Core;

using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using Entities;

namespace Bl.Interfaces;

public interface ISettingsBl : ICrudBl<Setting, int, SettingsSearchParams, SettingsConvertParams>
{
    Task<Setting?> GetAsync(SettingType settingType);
}

