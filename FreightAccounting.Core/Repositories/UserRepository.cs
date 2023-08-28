using FreightAccounting.Core.Common;
using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Users;
using Microsoft.EntityFrameworkCore;
namespace FreightAccounting.Core.Repositories;


public class UserRepository : IUserRepository
{
    private readonly FreightAccountingContext _context;

    public UserRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.AsNoTracking().ToArrayAsync();
    }

    public async Task AddUser(AddUserModel userModel)
    {
        await _context.Users.AddAsync(new User
        {
            NameAndFamily = userModel.NameAndFamily,
            Password = PasswordHasher.HashPassword(userModel.Password),
            Username = userModel.Username,
        });

        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// ویرایش اطلاعات کاربر
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userModel"></param>
    /// <returns></returns>
    public async Task UpdateUserInfo(int userId, UpdateUserInfoModel userModel)
    {
        var user = await GetUserById(userId);

        user.Username = userModel.Username;
        user.NameAndFamily = userModel.NameAndFamily;

        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// ویرایش پسورد توسط عادی
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="passwordModel"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    public async Task UpdateUserPassword(int userId, UpdateUserPasswordModel passwordModel)
    {
        var user = await GetUserById(userId);

        if (user.Password == PasswordHasher.HashPassword(passwordModel.OldPassword))
        {
            user.Password = PasswordHasher.HashPassword(passwordModel.NewPassowrd);
        }
        else
        {
            throw new AppException("گذرواژه قبلی اشتباه است!");
        }
    }
    /// <summary>
    /// ویرایش پسورد توسط کاربر روت
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task UpdatePasswordByAdmin(int userId, string password)
    {
        var user = await GetUserById(userId);
        user.Password = PasswordHasher.HashPassword(PasswordHasher.HashPassword(password));
    }

    public async Task DeleteUser(int userId)
    {
        var user = await GetUserById(userId);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private async Task<User> GetUserById(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(d => d.Id == userId);
        if (user is null)
        {
            throw new AppException("کاربر مورد نظر یافت نشد");
        }
        return user;
    }
}
