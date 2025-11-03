using Dal.DbModels.Core;

namespace Dal.DbModels;

public sealed class User : BaseDbModel<int>
{
    public int ApplicationUserId { get; set; }
}
