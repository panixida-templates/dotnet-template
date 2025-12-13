using Bl.Interfaces;

using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;
using Common.SearchParams.Core;

using Dal.Interfaces;

using Entities;

namespace Bl.Implementations;

public sealed class SettingsBl : ISettingsBl
{
    private readonly ISettingsDal _settingsDal;

    public SettingsBl(ISettingsDal settingsDal)
    {
        _settingsDal = settingsDal;
    }

    public Task<Setting> GetAsync(int id, SettingsConvertParams? convertParams = null)
    {
        return _settingsDal.GetAsync(id, convertParams);
    }

    public Task<SearchResult<Setting>> GetAsync(SettingsSearchParams searchParams, SettingsConvertParams? convertParams = null)
    {
        return _settingsDal.GetAsync(searchParams, convertParams);
    }

    public Task<Setting?> GetAsync(SettingType settingType)
    {
        return _settingsDal.GetAsync(settingType);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _settingsDal.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(SettingsSearchParams searchParams)
    {
        return _settingsDal.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(Setting entity)
    {
        entity.Id = await _settingsDal.AddOrUpdateAsync(entity);
        return entity.Id;
    }

    public async Task<IList<int>> AddOrUpdateAsync(IList<Setting> entities)
    {
        return await _settingsDal.AddOrUpdateAsync(entities);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _settingsDal.DeleteAsync(id);
    }

    public Task<bool> DeleteAsync(List<int> ids)
    {
        return _settingsDal.DeleteAsync(db => ids.Contains(db.Id));
    }
}
