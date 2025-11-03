namespace Api.Infrastructure.Models.Core;

public abstract record BaseModel<TId>
{
    public required TId Id { get; set; }
}
