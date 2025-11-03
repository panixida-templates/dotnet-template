namespace Dal.DbModels.Core;

public abstract class BaseDbModel<TId> : IBaseDbModel<TId>
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
