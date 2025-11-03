using System.Linq.Expressions;

using Common.SearchParams.Core;

namespace Dal.Interfaces.Core;

public interface IBaseDal<TDbObject, TEntity, TObjectId, TSearchParams, TConvertParams>
    where TDbObject : class, new()
    where TEntity : class
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    Task<TEntity> GetAsync(TObjectId id, TConvertParams? convertParams = null);
    Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null);
    Task<bool> ExistsAsync(TObjectId id);
    Task<bool> ExistsAsync(TSearchParams searchParams);
    Task<TObjectId> AddOrUpdateAsync(TEntity entity);
    Task<IList<TObjectId>> AddOrUpdateAsync(IList<TEntity> entities);
    Task<bool> DeleteAsync(TObjectId id);
    Task<bool> DeleteAsync(Expression<Func<TDbObject, bool>> predicate);
}