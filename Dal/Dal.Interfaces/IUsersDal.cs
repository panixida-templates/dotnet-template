using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces.Core;

namespace Dal.Interfaces;

public interface IUsersDal : IBaseDal<DbModels.User, Entities.User, int, UsersSearchParams, UsersConvertParams>
{
}