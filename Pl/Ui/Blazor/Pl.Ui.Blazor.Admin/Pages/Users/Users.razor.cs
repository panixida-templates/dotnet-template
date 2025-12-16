using Common.SearchParams;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using Pl.Ui.Blazor.Admin.Extensions;
using Pl.Ui.Blazor.Admin.Pages.Core;
using Pl.Ui.Blazor.Services.Interfaces;
using Pl.Ui.Blazor.ViewModels;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class Users : BaseFilterableTable<UserViewModel, UsersSearchParams>
{
    [Inject] private IUsersService UsersService { get; set; } = default!;

    private async Task<TableData<UserViewModel>> LoadServerData(TableState state, CancellationToken cancellationToken)
    {
        _loading = true;

        try
        {
            _currentPage = state.Page;
            _searchParams.ApplyTableState(state);

            var result = await UsersService.GetAsync(_searchParams, cancellationToken: cancellationToken);

            return new TableData<UserViewModel>
            {
                TotalItems = result.Total,
                Items = result.Objects
            };
        }
        finally
        {
            _loading = false;
        }
    }
}
