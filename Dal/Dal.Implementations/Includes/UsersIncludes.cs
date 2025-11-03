using Common.ConvertParams;

using Dal.DbModels;

namespace Dal.Implementations.Includes;

internal static class UsersIncludes
{
    internal static IQueryable<User> Include(this IQueryable<User> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
