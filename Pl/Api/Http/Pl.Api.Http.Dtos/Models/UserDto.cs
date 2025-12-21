using Common.Enums;

using Pl.Api.Http.Dtos.Models.Core;

namespace Pl.Api.Http.Dtos.Models;

public sealed record UserDto : BaseDto<int>
{
    public required Role Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required int Age { get; set; }
    public required DateTime Birthday { get; set; }
}
