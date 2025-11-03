using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.DbModels.Core;
using Dal.Interfaces.Core;

using Entities.Core;

using Microsoft.EntityFrameworkCore;

namespace Dal.Implementations.Core;

public abstract class BaseDal<TDbContext, TDbObject, TEntity, TId, TSearchParams, TConvertParams>
    : IBaseDal<TDbObject, TEntity, TId, TSearchParams, TConvertParams>
    where TDbContext : DbContext
    where TDbObject : class, IBaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>, IComparable<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    protected TDbContext _context;

    protected abstract bool RequiresUpdatesAfterObjectSaving { get; }

    private protected BaseDal(TDbContext context)
    {
        _context = context;
    }

    public virtual Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return AddOrUpdateAsync(entity, true);
    }

    internal virtual async Task<TId> AddOrUpdateAsync(TEntity entity, bool forceSave)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var data = _context;
        var objects = data.Set<TDbObject>();
        var dbObject = await objects.FirstOrDefaultAsync(item => item.Id.Equals(entity.Id));
        var exists = dbObject != null;
        dbObject ??= new TDbObject();

        await UpdateBeforeSavingAsync(entity, dbObject);

        var now = DateTime.UtcNow;
        if (!exists)
        {
            dbObject.CreatedAt = now;
            objects.Add(dbObject);
        }
        dbObject.UpdatedAt = now;

        if (RequiresUpdatesAfterObjectSaving || forceSave)
        {
            await data.SaveChangesAsync();
        }

        if (RequiresUpdatesAfterObjectSaving)
        {
            await UpdateAfterSavingAsync(entity, dbObject);

            if (forceSave)
            {
                await data.SaveChangesAsync();
            }
        }
        entity.Id = dbObject.Id;

        return dbObject.Id;
    }

    public virtual Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return AddOrUpdateAsync(entities, true);
    }

    internal virtual async Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities, bool forceSave)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var data = _context;
        var entitiesIdArray = entities.Select(item => item.Id).ToArray();
        var dbSet = data.Set<TDbObject>();
        var dbObjectsDictionary = await dbSet.Where(item => entitiesIdArray.Any(id => item.Id.Equals(id))).ToDictionaryAsync(item => item.Id);

        var existingSet = new HashSet<TId>();
        var dbObjects = new List<TDbObject>();
        var addedObjects = new List<TDbObject>();

        foreach (var entity in entities)
        {
            var id = entity.Id;
            var exists = dbObjectsDictionary.TryGetValue(id, out var dbObject);
            dbObject ??= new TDbObject();

            if (exists)
            {
                existingSet.Add(id);
            }

            var now = DateTime.UtcNow;
            if (!exists)
            {
                dbObject.CreatedAt = now;
            }
            dbObject.UpdatedAt = now;

            await UpdateBeforeSavingAsync(entity, dbObject);
            dbObjects.Add(dbObject);

            if (!exists)
            {
                addedObjects.Add(dbObject);
            }
        }

        dbSet.AddRange(addedObjects);

        if (RequiresUpdatesAfterObjectSaving || forceSave)
        {
            await data.SaveChangesAsync();
        }

        if (RequiresUpdatesAfterObjectSaving)
        {
            for (var i = 0; i < dbObjects.Count; i++)
            {
                var dbObject = dbObjects[i];
                await UpdateAfterSavingAsync(entities[i], dbObject);
            }
        }

        if (forceSave)
        {
            await data.SaveChangesAsync();
        }

        return [.. dbObjects.Select(item => item.Id)];
    }

    public virtual Task<bool> ExistsAsync(TId id)
    {
        return _context.Set<TDbObject>().Where(item => item.Id.Equals(id)).AnyAsync();
    }

    public virtual async Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var data = _context;
        var objects = data.Set<TDbObject>().AsNoTracking();

        return await (await BuildDbQueryAsync(objects, searchParams)).AnyAsync();
    }

    internal virtual async Task<bool> ExistsAsync(Expression<Func<TDbObject, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var data = _context;

        return await data.Set<TDbObject>().Where(predicate).AnyAsync();
    }

    public virtual Task<bool> DeleteAsync(TId id)
    {
        return DeleteAsync(item => item.Id.Equals(id));
    }

    public virtual async Task<bool> DeleteAsync(Expression<Func<TDbObject, bool>> predicate)
    {
        var count = await _context.Set<TDbObject>()
            .Where(predicate)
            .Where(item => item.DeletedAt == null)
            .ExecuteUpdateAsync(item => item.SetProperty(prop => prop.DeletedAt, prop => DateTime.UtcNow));

        return count != 0;
    }

    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams = convertParams ?? new TConvertParams();

        var data = _context;
        var dbObjects = data.Set<TDbObject>().Where(item => item.Id.Equals(id) && item.DeletedAt.Equals(null)).Take(1);

        return (await BuildEntitiesListAsync(dbObjects, convertParams)).FirstOrDefault() ?? throw new NotFoundException($"{typeof(TDbObject).Name} с id={id} не найдена");
    }

    public virtual async Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        convertParams = convertParams ?? new TConvertParams();

        var data = _context;
        var objects = data.Set<TDbObject>().AsNoTracking();

        objects = await BuildDbQueryAsync(objects, searchParams);

        var mappedSortField = MapSortField(searchParams.SortField ?? string.Empty);
        if (!string.IsNullOrEmpty(searchParams.SortField) && !string.IsNullOrEmpty(mappedSortField))
        {
            if (searchParams.SortOrder == SortOrder.Descending)
            {
                objects = objects.OrderBy($"{mappedSortField} descending");
            }
            else
            {
                objects = objects.OrderBy(mappedSortField);
            }
        }
        else
        {
            var visitor = new OrderedQueryableVisitor();
            visitor.Visit(objects.Expression);
            if (visitor.IsOrdered && objects is IOrderedQueryable<TDbObject> orderedObjects)
            {
                objects = orderedObjects
                    .ThenByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
            else
            {
                objects = objects
                    .OrderByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
        }

        var result = new SearchResult<TEntity>
        {
            Total = await objects.CountAsync(),
            Objects = [],
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };

        if (searchParams.ObjectsCount == 0)
        {
            return result;
        }

        objects = objects.Skip((searchParams.Page - 1) * (searchParams.ObjectsCount ?? 0));
        if (searchParams.ObjectsCount.HasValue)
        {
            objects = objects.Take(searchParams.ObjectsCount.Value);
        }
        result.Objects = await BuildEntitiesListAsync(objects, convertParams);

        return result;
    }

    internal virtual async Task<IList<TEntity>> GetAsync(Expression<Func<TDbObject, bool>> predicate, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        convertParams = convertParams ?? new TConvertParams();
        var data = _context;

        return await BuildEntitiesListAsync(data.Set<TDbObject>().Where(predicate), convertParams);
    }

    protected abstract Task UpdateBeforeSavingAsync(TEntity entity, TDbObject dbObject);

    protected virtual Task UpdateAfterSavingAsync(TEntity entity, TDbObject dbObject)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<IQueryable<TDbObject>> BuildDbQueryAsync(IQueryable<TDbObject> dbObjects, TSearchParams searchParams)
    {
        if (searchParams.IsDeleted)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt != null);
        }
        else
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt == null);
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.CreatedFrom <= item.CreatedAt);
        }
        if (searchParams.CreatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.CreatedAt <= searchParams.CreatedTo);
        }
        if (searchParams.UpdatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.UpdatedFrom <= item.UpdatedAt);
        }
        if (searchParams.UpdatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.UpdatedAt <= searchParams.UpdatedTo);
        }
        if (searchParams.DeletedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.DeletedFrom <= item.DeletedAt);
        }
        if (searchParams.DeletedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt <= searchParams.DeletedTo);
        }

        return Task.FromResult(dbObjects);
    }

    protected abstract Task<IList<TEntity>> BuildEntitiesListAsync(IQueryable<TDbObject> dbObjects, TConvertParams convertParams);

    protected virtual string MapSortField(string sortField)
    {
        return sortField;
    }
}