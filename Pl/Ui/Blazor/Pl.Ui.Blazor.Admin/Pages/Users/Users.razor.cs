using Common.Enums;
using Common.SearchParams;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using Pl.Ui.Blazor.Services.Interfaces;
using Pl.Ui.Blazor.ViewModels;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class Users
{
    [Inject] private IUsersService UsersService { get; set; } = default!;

    private MudTable<UserViewModel>? _table;

    private bool _loading;
    private bool _filtersExpanded;
    private int _currentPage;

    private UsersSearchParams _searchParams = new();

    private Task OnFiltersExpandedChanged(bool expanded)
    {
        _filtersExpanded = expanded;
        return Task.CompletedTask;
    }

    private async Task OnFiltersChanged()
    {
        if (_table is null)
        {
            return;
        }
        if (_currentPage != 0)
        {
            _table.NavigateTo(Page.First);
            return;
        }

        await _table.ReloadServerData();
    }

    private async Task ResetFilters()
    {
        _searchParams = new();
        await OnFiltersChanged();
    }

    private async Task<TableData<UserViewModel>> LoadServerData(TableState state, CancellationToken cancellationToken)
    {
        _loading = true;

        try
        {
            _currentPage = state.Page;

            CreateSearchParams(state);

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

    private void CreateSearchParams(TableState state)
    {
        _searchParams.Page = state.Page + 1;
        _searchParams.ObjectsCount = state.PageSize;

        ApplySorting(_searchParams, state);
    }

    private static void ApplySorting(UsersSearchParams searchParams, TableState state)
    {
        if (state.SortDirection == SortDirection.None)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(state.SortLabel))
        {
            return;
        }

        searchParams.SortField = state.SortLabel;
        searchParams.SortOrder = MapSortOrder(state.SortDirection);
    }

    private static SortOrder MapSortOrder(SortDirection direction)
    {
        if (direction == SortDirection.Descending)
        {
            return SortOrder.Descending;
        }
        if (direction == SortDirection.Ascending)
        {
            return SortOrder.Ascending;
        }

        return default;
    }
}
