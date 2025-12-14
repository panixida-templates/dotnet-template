using MudBlazor;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class Users
{
    private MudTable<UserRow>? _table;

    private string _search = string.Empty;
    private string? _role;
    private bool? _isActive;

    private bool _loading;

    private readonly List<string> _roles = ["Admin", "Manager", "Operator", "Viewer"];
    private readonly List<UserRow> _all = TestData.GenerateUsers(250);

    private Task OnFiltersChanged()
        => _table?.ReloadServerData() ?? Task.CompletedTask;

    private async Task ResetFilters()
    {
        _search = string.Empty;
        _role = null;
        _isActive = null;

        await OnFiltersChanged();
    }

    private Task<TableData<UserRow>> LoadServerData(TableState state, CancellationToken ct)
    {
        _loading = true;

        IEnumerable<UserRow> query = _all;

        if (!string.IsNullOrWhiteSpace(_search))
        {
            var s = _search.Trim();
            query = query.Where(x =>
                x.FullName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(_role))
            query = query.Where(x => x.Role == _role);

        if (_isActive.HasValue)
            query = query.Where(x => x.IsActive == _isActive.Value);

        query = ApplySorting(query, state);

        var total = query.Count();

        query = query
            .Skip(state.Page * state.PageSize)
            .Take(state.PageSize);

        _loading = false;

        return Task.FromResult(new TableData<UserRow>
        {
            TotalItems = total,
            Items = query.ToList()
        });
    }

    private static IEnumerable<UserRow> ApplySorting(IEnumerable<UserRow> query, TableState state)
    {
        if (state.SortDirection == SortDirection.None || string.IsNullOrWhiteSpace(state.SortLabel))
            return query.OrderBy(x => x.FullName);

        return (state.SortLabel, state.SortDirection) switch
        {
            ("name", SortDirection.Ascending) => query.OrderBy(x => x.FullName),
            ("name", SortDirection.Descending) => query.OrderByDescending(x => x.FullName),

            ("email", SortDirection.Ascending) => query.OrderBy(x => x.Email),
            ("email", SortDirection.Descending) => query.OrderByDescending(x => x.Email),

            ("role", SortDirection.Ascending) => query.OrderBy(x => x.Role),
            ("role", SortDirection.Descending) => query.OrderByDescending(x => x.Role),

            ("created", SortDirection.Ascending) => query.OrderBy(x => x.CreatedAt),
            ("created", SortDirection.Descending) => query.OrderByDescending(x => x.CreatedAt),

            ("active", SortDirection.Ascending) => query.OrderBy(x => x.IsActive),
            ("active", SortDirection.Descending) => query.OrderByDescending(x => x.IsActive),

            _ => query.OrderBy(x => x.FullName)
        };
    }

    private sealed record UserRow(
        string FullName,
        string Email,
        string Role,
        DateTime CreatedAt,
        bool IsActive);

    private static class TestData
    {
        private static readonly string[] FirstNames =
        [
            "Алексей","Иван","Дмитрий","Сергей","Михаил","Андрей","Павел","Никита","Егор","Кирилл",
            "Анна","Мария","Екатерина","Ольга","Наталья","Алина","Полина","Виктория","София","Ксения"
        ];

        private static readonly string[] LastNames =
        [
            "Иванов","Петров","Сидоров","Смирнов","Кузнецов","Попов","Васильев","Новиков","Фёдоров","Морозов"
        ];

        public static List<UserRow> GenerateUsers(int count)
        {
            var rnd = new Random(42);
            var roles = new[] { "Admin", "Manager", "Operator", "Viewer" };

            var list = new List<UserRow>(count);

            for (var i = 0; i < count; i++)
            {
                var first = FirstNames[rnd.Next(FirstNames.Length)];
                var last = LastNames[rnd.Next(LastNames.Length)];

                list.Add(new UserRow(
                    FullName: $"{last} {first}",
                    Email: $"{last.ToLowerInvariant()}.{first.ToLowerInvariant()}{i}@example.com",
                    Role: roles[rnd.Next(roles.Length)],
                    CreatedAt: DateTime.UtcNow.Date.AddDays(-rnd.Next(0, 365)),
                    IsActive: rnd.NextDouble() > 0.2
                ));
            }

            return list;
        }
    }
}
