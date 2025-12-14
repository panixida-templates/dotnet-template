using Pl.Api.Http.Dtos.Models;
using Pl.Ui.Blazor.ViewModels;

namespace Pl.Ui.Blazor.Mappers;

public static class UsersMapper
{
    public static UserViewModel ToViewModel(this UserDto dto)
    {
        return new UserViewModel
        {
            Id = dto.Id,
            Role = dto.Role,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Age = dto.Age,
            Birthday = dto.Birthday
        };
    }

    public static List<UserViewModel> ToViewModel(this IEnumerable<UserDto> dtos)
    {
        return [.. dtos.Select(ToViewModel)];
    }

    public static UserDto ToDto(this UserViewModel viewModel)
    {
        return new UserDto
        {
            Id = viewModel.Id,
            Role = viewModel.Role,
            Name = viewModel.Name,
            Email = viewModel.Email,
            Phone = viewModel.Phone,
            Age = viewModel.Age,
            Birthday = viewModel.Birthday
        };
    }

    public static List<UserDto> ToDto(this IEnumerable<UserViewModel> viewModels)
    {
        return [.. viewModels.Select(ToDto)];
    }
}
