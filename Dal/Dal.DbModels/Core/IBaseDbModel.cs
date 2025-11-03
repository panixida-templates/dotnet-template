namespace Dal.DbModels.Core;

public interface IBaseDbModel<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime? DeletedAt { get; set; }
}