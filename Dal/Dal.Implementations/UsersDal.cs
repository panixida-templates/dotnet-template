using Common.ConvertParams;
using Common.SearchParams;

using Dal.Ef;
using Dal.Implementations.Core;
using Dal.Implementations.Filters;
using Dal.Implementations.Includes;
using Dal.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Dal.Implementations;

public sealed class UsersDal : BaseDal<DefaultDbContext, DbModels.User, Entities.User, int, UsersSearchParams, UsersConvertParams>, IUsersDal
{
    protected override bool RequiresUpdatesAfterObjectSaving => false;

    public UsersDal(DefaultDbContext context) : base(context)
    {
    }

    protected override Task UpdateBeforeSavingAsync(Entities.User entity, DbModels.User dbObject)
    {
        dbObject.Role = entity.Role;
        dbObject.Name = entity.Name;
        dbObject.Email = entity.Email;
        dbObject.Phone = entity.Phone;
        dbObject.Age = entity.Age;
        dbObject.Birthday = entity.Birthday;

        return Task.CompletedTask;
    }

    protected override async Task<IQueryable<DbModels.User>> BuildDbQueryAsync(IQueryable<DbModels.User> dbObjects, UsersSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Entities.User>> BuildEntitiesListAsync(IQueryable<DbModels.User> dbObjects, UsersConvertParams convertParams)
    {
        return (await dbObjects.Include(convertParams).ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
    }

    internal static Entities.User ConvertDbObjectToEntity(DbModels.User dbObject)
    {
        return new Entities.User(
            id: dbObject.Id,
            role: dbObject.Role,
            name: dbObject.Name,
            email: dbObject.Email,
            phone: dbObject.Phone,
            age: dbObject.Age,
            birthday: dbObject.Birthday)
        {
        };
    }
}
