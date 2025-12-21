using Entities;

using Pl.Api.Http.Dtos.Models;

namespace Pl.Api.Http.Mappers;

public static class SettingsMapper
{
    public static Setting ToEntity(this SettingDto dto)
    {
        var entity = new Setting(
            id: dto.Id,
            settingType: dto.SettingType,
            value: dto.Value
        );

        return entity;
    }

    public static List<Setting> ToEntity(this IEnumerable<SettingDto> dtos)
    {
        return [.. dtos.Select(ToEntity)];
    }

    public static SettingDto ToDto(this Setting entity)
    {
        var dto = new SettingDto
        {
            Id = entity.Id,
            SettingType = entity.SettingType,
            Value = entity.Value
        };

        return dto;
    }

    public static List<SettingDto> ToDto(this IEnumerable<Setting> entities)
    {
        return [.. entities.Select(ToDto)];
    }
}
