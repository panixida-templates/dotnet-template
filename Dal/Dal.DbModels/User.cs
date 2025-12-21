using Common.Enums;

using Dal.DbModels.Core;

namespace Dal.DbModels;

public sealed class User : BaseDbModel<int>
{
    public Role Role { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public int Age { get; set; }
    public DateTime Birthday { get; set; }
}
