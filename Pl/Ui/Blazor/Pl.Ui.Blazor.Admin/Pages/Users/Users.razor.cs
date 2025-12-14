using Common.ConvertParams;
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

    private string _search = string.Empty;
    private Role? _role;

    private bool _loading;

    private readonly List<Role> _roles = GetAllRoles();

    private readonly UsersConvertParams _convertParams = new();

    private Task OnFiltersChanged()
    {
        if (_table is null)
        {
            return Task.CompletedTask;
        }

        return _table.ReloadServerData();
    }

    private async Task ResetFilters()
    {
        _search = string.Empty;
        _role = null;

        await OnFiltersChanged();
    }

    private async Task<TableData<UserViewModel>> LoadServerData(TableState state, CancellationToken ct)
    {
        _loading = true;

        try
        {
            var searchParams = CreateSearchParams(state);

            var result = await UsersService.GetAsync(
                searchParams: searchParams,
                convertParams: _convertParams,
                cancellationToken: ct);

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

    private UsersSearchParams CreateSearchParams(TableState state)
    {
        var searchParams = new UsersSearchParams();

        // ВАЖНО:
        // MudTable state.Page — 0-based.
        // Если ваш API ожидает 1-based страницу, замените на: state.Page + 1
        searchParams.Page = state.Page;
        searchParams.ObjectsCount = state.PageSize;

        if (!string.IsNullOrWhiteSpace(_search))
        {
            searchParams.Search = _search.Trim();
        }

        if (_role.HasValue)
        {
            searchParams.Role = _role.Value;
        }

        ApplySorting(searchParams, state);

        return searchParams;
    }

    private static void ApplySorting(UsersSearchParams searchParams, TableState state)
    {
        if (state.SortDirection == MudBlazor.SortDirection.None || string.IsNullOrWhiteSpace(state.SortLabel))
        {
            return;
        }

        // Подстройте под ваш контракт сортировки.
        // Вариант А: если на бэке SortField = string, SortDesc = bool:
        searchParams.SortField = MapSortField(state.SortLabel);
        searchParams.SortDesc = state.SortDirection == MudBlazor.SortDirection.Descending;

        // Вариант Б: если у вас SortDirection enum/строка — замените присваивания под вашу модель.
    }

    private static string MapSortField(string sortLabel)
    {
        // Здесь важно, чтобы значения совпадали с тем, что понимает сервер.
        // Если сервер ждёт "Name"/"Email" — возвращайте их.
        return sortLabel switch
        {
            "name" => "name",
            "email" => "email",
            "role" => "role",
            "phone" => "phone",
            "age" => "age",
            "birthday" => "birthday",
            _ => "name"
        };
    }

    private static List<Role> GetAllRoles()
    {
        var list = new List<Role>();

        foreach (var value in Enum.GetValues<Role>())
        {
            list.Add(value);
        }

        return list;
    }
}
