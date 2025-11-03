using Common.SearchParams;

using Dal.DbModels;

namespace Dal.Implementations.Filters;

internal static class UsersFilters
{
    internal static IQueryable<User> Filter(this IQueryable<User> dbObjects, UsersSearchParams searchParams)
    {
        return dbObjects;
    }
}
