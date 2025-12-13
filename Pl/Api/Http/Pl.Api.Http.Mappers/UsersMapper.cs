using Entities;

using Pl.Api.Http.Dtos.Models;

namespace Pl.Api.Http.Mappers;

public static class UsersMapper
{
    public static User ToEntity(this UserDto dto)
    {
        var entity = new User(
            id: dto.Id,
            applicationUserId: dto.Id
        );

        return entity;
    }

    public static List<User> ToEntity(this IEnumerable<UserDto> dtos)
    {
        return [.. dtos.Select(ToEntity)];
    }

    public static UserDto ToDto(this User entity)
    {
        var dto = new UserDto
        {
            Id = entity.Id,
            Name = string.Empty
        };

        return dto;
    }

    public static List<UserDto> ToDto(this IEnumerable<User> entities)
    {
        return [.. entities.Select(ToDto)];
    }
}
