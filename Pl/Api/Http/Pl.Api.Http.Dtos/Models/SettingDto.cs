using Common.Enums;

using Pl.Api.Http.Dtos.Models.Core;

namespace Pl.Api.Http.Dtos.Models;

public sealed record SettingDto : BaseDto<int>
{
    public required SettingType SettingType { get; set; }
    public required string Value { get; set; }
}
