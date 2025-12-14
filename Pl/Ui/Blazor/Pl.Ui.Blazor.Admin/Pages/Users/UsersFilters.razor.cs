using Microsoft.AspNetCore.Components;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class UsersFilters
{
    [Parameter] public string Search { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> SearchChanged { get; set; }

    [Parameter] public string? Role { get; set; }
    [Parameter] public EventCallback<string?> RoleChanged { get; set; }

    [Parameter] public bool? IsActive { get; set; }
    [Parameter] public EventCallback<bool?> IsActiveChanged { get; set; }

    [Parameter] public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

    [Parameter] public EventCallback OnChanged { get; set; }
    [Parameter] public EventCallback OnReset { get; set; }

    private Task OnSearchChanged(string value)
        => SearchChanged.InvokeAsync(value);

    private Task OnSearchDebounced(string _)
        => OnChanged.InvokeAsync();

    private async Task OnRoleChanged(string? value)
    {
        await RoleChanged.InvokeAsync(value);
        await OnChanged.InvokeAsync();
    }

    private async Task OnIsActiveChanged(bool? value)
    {
        await IsActiveChanged.InvokeAsync(value);
        await OnChanged.InvokeAsync();
    }

    private Task OnResetClicked()
        => OnReset.InvokeAsync();
}
