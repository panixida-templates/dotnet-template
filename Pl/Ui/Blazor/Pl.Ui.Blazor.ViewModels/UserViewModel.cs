using Common.Enums;

using Pl.Ui.Blazor.ViewModels.Core;

namespace Pl.Ui.Blazor.ViewModels;

public sealed record UserViewModel : BaseViewModel<int>
{
    public required Role Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required int Age { get; set; }
    public required DateTime Birthday { get; set; }
}
