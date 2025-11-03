using Common.ConvertParams;

using Dal.DbModels;

namespace Dal.Implementations.Includes;

internal static class SettingsIncludes
{
    internal static IQueryable<Settings> Include(this IQueryable<Settings> dbObjects, SettingsConvertParams convertParams)
    {
        return dbObjects;
    }
}
