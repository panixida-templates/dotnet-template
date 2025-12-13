using Pl.Api.Http.Dtos.Models.Core;

namespace Pl.Api.Http.Dtos.Models;

public sealed record UserDto : BaseDto<int>
{
    public required string Name { get; set; }
}
