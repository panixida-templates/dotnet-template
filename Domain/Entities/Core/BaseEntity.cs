namespace Entities.Core;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; }

    protected BaseEntity(TId id)
    {
        Id = id;
    }
}
