using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model.Users;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUser(AddUserModel userModel);
    Task DeleteUser(int userId);
    Task<IEnumerable<User>> GetUsers();
    Task UpdatePasswordByAdmin(int userId, string password);
    Task UpdateUserInfo(int userId, UpdateUserInfoModel userModel);
    Task UpdateUserPassword(int userId, UpdateUserPasswordModel passwordModel);
}
