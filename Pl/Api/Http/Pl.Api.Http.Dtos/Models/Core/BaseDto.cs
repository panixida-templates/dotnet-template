namespace Pl.Api.Http.Dtos.Models.Core;

public abstract record BaseDto<TId>
{
    public required TId Id { get; set; }
}
