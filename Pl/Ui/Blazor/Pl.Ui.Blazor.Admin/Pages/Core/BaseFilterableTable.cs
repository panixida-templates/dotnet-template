using Common.SearchParams.Core;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace Pl.Ui.Blazor.Admin.Pages.Core;

public abstract class BaseFilterableTable<TItem, TSearchParams> : ComponentBase
    where TSearchParams : BaseSearchParams, new()
{
    protected MudTable<TItem>? _table;

    protected bool _loading;
    protected bool _filtersExpanded;
    protected int _currentPage;

    protected TSearchParams _searchParams = new();

    protected Task OnFiltersExpandedChanged(bool expanded)
    {
        _filtersExpanded = expanded;
        return Task.CompletedTask;
    }

    protected async Task OnFiltersChanged()
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

    protected async Task ResetFilters()
    {
        _searchParams = new TSearchParams();
        await OnFiltersChanged();
    }
}
