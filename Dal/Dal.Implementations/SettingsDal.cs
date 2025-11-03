using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using Dal.Ef;
using Dal.Implementations.Core;
using Dal.Implementations.Filters;
using Dal.Implementations.Includes;
using Dal.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Dal.Implementations;

public sealed class SettingsDal : BaseDal<DefaultDbContext, DbModels.Settings, Entities.Settings, int, SettingsSearchParams, SettingsConvertParams>, ISettingsDal
{
    protected override bool RequiresUpdatesAfterObjectSaving => false;

    public SettingsDal(DefaultDbContext context) : base(context)
    {
    }

    protected override Task UpdateBeforeSavingAsync(Entities.Settings entity, DbModels.Settings dbObject)
    {
        dbObject.SettingType = entity.SettingType;
        dbObject.Value = entity.Value;

        return Task.CompletedTask;
    }

    protected override async Task<IQueryable<DbModels.Settings>> BuildDbQueryAsync(IQueryable<DbModels.Settings> dbObjects, SettingsSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Entities.Settings>> BuildEntitiesListAsync(IQueryable<DbModels.Settings> dbObjects, SettingsConvertParams convertParams)
    {
        return (await dbObjects.Include(convertParams).ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
    }

    internal static Entities.Settings ConvertDbObjectToEntity(DbModels.Settings dbObject)
    {
        return new Entities.Settings(
            id: dbObject.Id,
            settingType: dbObject.SettingType,
            value: dbObject.Value);
    }

    public async Task<Entities.Settings?> GetAsync(SettingType settingType)
    {
        return (await GetAsync(item => item.SettingType == settingType)).FirstOrDefault();
    }
}
