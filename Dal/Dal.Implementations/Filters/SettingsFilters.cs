using Common.SearchParams;

using Dal.DbModels;

namespace Dal.Implementations.Filters;

internal static class SettingsFilters
{
    internal static IQueryable<Settings> Filter(this IQueryable<Settings> dbObjects, SettingsSearchParams searchParams)
    {
        return dbObjects;
    }
}
