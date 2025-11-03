using Bl.Interfaces;

using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Dal.Interfaces;

using Entities;

namespace Bl.Implementations;

public sealed class UsersBl : IUsersBl
{
    private readonly IUsersDal _usersDal;

    public UsersBl(IUsersDal usersDal)
    {
        _usersDal = usersDal;
    }

    public Task<User> GetAsync(int id, UsersConvertParams? convertParams = null)
    {
        return _usersDal.GetAsync(id, convertParams);
    }

    public Task<SearchResult<User>> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null)
    {
        return _usersDal.GetAsync(searchParams, convertParams);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _usersDal.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(UsersSearchParams searchParams)
    {
        return _usersDal.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(User entity)
    {
        entity.Id = await _usersDal.AddOrUpdateAsync(entity);
        return entity.Id;
    }

    public Task<IList<int>> AddOrUpdateAsync(IList<User> entities)
    {
        return _usersDal.AddOrUpdateAsync(entities);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _usersDal.DeleteAsync(id);
    }

    public Task<bool> DeleteAsync(List<int> ids)
    {
        return _usersDal.DeleteAsync(db => ids.Contains(db.Id));
    }
}

