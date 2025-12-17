using Common.Enums;

using Pl.Ui.Blazor.ViewModels.Core;

using System.ComponentModel.DataAnnotations;

namespace Pl.Ui.Blazor.ViewModels;

public sealed record UserViewModel : BaseViewModel<int>
{
    [Display(Name = "Роль")]
    public required Role Role { get; set; }

    [Display(Name = "Имя")]
    public required string Name { get; set; }

    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Display(Name = "Телефон")]
    public required string Phone { get; set; }

    [Display(Name = "Возраст")]
    public required int Age { get; set; }

    [Display(Name = "День рождения")]
    public required DateTime Birthday { get; set; }
}
