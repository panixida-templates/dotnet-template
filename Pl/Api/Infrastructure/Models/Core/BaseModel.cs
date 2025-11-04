using Swashbuckle.AspNetCore.Annotations;

namespace Api.Infrastructure.Models.Core;

public abstract record BaseModel<TId>
{
    [SwaggerSchema(ReadOnly = true)]
    public required TId Id { get; set; }
}
