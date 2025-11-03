using Common.SearchParams.Core;

namespace Bl.Interfaces.Core;

public interface ICrudBl<TEntity, TEntityId, TSearchParams, TConvertParams>
    where TEntity : class
    where TEntityId : notnull
    where TSearchParams : class
    where TConvertParams : class, new()
{
    Task<TEntity> GetAsync(TEntityId id, TConvertParams? convertParams = null);
    Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null);
    Task<bool> ExistsAsync(TEntityId id);
    Task<bool> ExistsAsync(TSearchParams searchParams);
    Task<TEntityId> AddOrUpdateAsync(TEntity entity);
    Task<IList<TEntityId>> AddOrUpdateAsync(IList<TEntity> entities);
    Task<bool> DeleteAsync(TEntityId id);
    Task<bool> DeleteAsync(List<TEntityId> ids);
}
