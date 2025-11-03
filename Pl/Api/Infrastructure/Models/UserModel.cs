using System.Collections.Generic;
using System.Linq;

using Api.Infrastructure.Models.Core;

using Gen.IdentityService.Entities;
using Gen.IdentityService.Enums;

namespace Api.Infrastructure.Models;

public sealed record UserModel : BaseModel<int>
{
    public required string Name { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required bool EmailConfirmed { get; set; }
    public required string PhoneNumber { get; set; }
    public required bool PhoneNumberConfirmed { get; set; }

    public required List<ApplicationUserRole> Roles { get; set; } = [];

    public static UserModel FromEntity(Entities.User obj)
    {
        return new UserModel
        {
            Id = obj.Id,
            Name = obj.ApplicationUser?.Name ?? string.Empty,
            Password = obj.ApplicationUser?.Password ?? string.Empty,
            Email = obj.ApplicationUser?.Email ?? string.Empty,
            EmailConfirmed = obj.ApplicationUser?.EmailConfirmed ?? false,
            PhoneNumber = obj.ApplicationUser?.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = obj.ApplicationUser?.PhoneNumberConfirmed ?? false,
            Roles = obj.ApplicationUser?.Roles.ToList() ?? [],
        };
    }

    public static Entities.User ToEntity(UserModel obj)
    {
        var entity = new Entities.User(
            id: obj.Id,
            applicationUserId: 0)
        {
            ApplicationUser = new ApplicationUser()
            {
                Id = 0,
                Name = obj.Name,
                Password = obj.Password,
                Email = obj.Email,
                EmailConfirmed = obj.EmailConfirmed,
                PhoneNumber = obj.PhoneNumber,
                PhoneNumberConfirmed = obj.PhoneNumberConfirmed,
            }
        };
        entity.ApplicationUser.Roles.AddRange(obj.Roles);

        return entity;
    }

    public static List<UserModel> FromEntitiesList(IEnumerable<Entities.User> list)
    {
        return list.Select(FromEntity).ToList()!;
    }

    public static List<Entities.User> ToEntitiesList(IEnumerable<UserModel> list)
    {
        return list.Select(ToEntity).ToList()!;
    }
}
