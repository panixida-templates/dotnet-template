using Entities.Core;

namespace Entities;

public sealed class User : BaseEntity<int>
{
    public int ApplicationUserId { get; set; }

    public User(
        int id,
        int applicationUserId) : base(id)
    {
        ApplicationUserId = applicationUserId;
    }
}
