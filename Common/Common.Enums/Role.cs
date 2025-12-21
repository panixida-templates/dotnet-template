using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum Role
{
    [Display(Name = "Разрабочтик")]
    Developer = 0,

    [Display(Name = "Администратор")]
    Admin = 1,

    [Display(Name = "Пользователь")]
    Client = 2,
}
