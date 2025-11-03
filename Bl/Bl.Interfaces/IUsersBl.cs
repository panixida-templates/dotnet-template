using Bl.Interfaces.Core;

using Common.ConvertParams;
using Common.SearchParams;

using Entities;

namespace Bl.Interfaces;

public interface IUsersBl : ICrudBl<User, int, UsersSearchParams, UsersConvertParams>
{
}