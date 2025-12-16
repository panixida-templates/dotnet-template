using Common.Enums;
using Common.SearchParams.Core;

using MudBlazor;

namespace Pl.Ui.Blazor.Admin.Extensions;

public static class BaseSearchParamsTableStateExtension
{
    public static void ApplyTableState(this BaseSearchParams searchParams, TableState state, IReadOnlyDictionary<string, string>? sortFieldMap = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        ArgumentNullException.ThrowIfNull(state);

        searchParams.Page = state.Page + 1;
        searchParams.ObjectsCount = state.PageSize;

        ApplySorting(searchParams, state, sortFieldMap);
    }

    private static void ApplySorting(BaseSearchParams searchParams, TableState state, IReadOnlyDictionary<string, string>? sortFieldMap)
    {
        if (state.SortDirection == SortDirection.None || string.IsNullOrWhiteSpace(state.SortLabel))
        {
            searchParams.SortField = null;
            searchParams.SortOrder = default;
            return;
        }

        var sortField = state.SortLabel;

        if (sortFieldMap is not null && sortFieldMap.TryGetValue(sortField, out var mapped))
        {
            sortField = mapped;
        }

        searchParams.SortField = sortField;
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
