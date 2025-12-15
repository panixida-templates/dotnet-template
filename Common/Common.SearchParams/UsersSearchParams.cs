using Common.Enums;
using Common.SearchParams.Core;

namespace Common.SearchParams;

public sealed class UsersSearchParams : BaseSearchParams
{
    public Role? Role { get; set; }

    public UsersSearchParams() : base() { }
}