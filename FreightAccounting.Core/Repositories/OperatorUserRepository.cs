using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core.Repositories;


public class OperatorUserRepository : IOperatorUserRepository
{
    private readonly FreightAccountingContext _context;

    public OperatorUserRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    public  IEnumerable<OperatorUser> GetOperatorUsers()
    {
        return _context.OperatorUsers.AsNoTracking().ToList();
    }

    public async Task AddOperatorUser(OperatorUser operatorUser)
    {
        await _context.OperatorUsers.AddAsync(operatorUser);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateOperatorUser(int userId, OperatorUser operatorUser)
    {
        var user = await GetOperatorUserById(userId);

        user.Name = operatorUser.Name;
        user.Family = operatorUser.Family;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteOperatorUser(int userId)
    {
        var user = await GetOperatorUserById(userId);

        _context.OperatorUsers.Remove(user);
        await _context.SaveChangesAsync();
    }

    private async Task<OperatorUser> GetOperatorUserById(int userId)
    {
        var user = await _context.OperatorUsers.FirstOrDefaultAsync(d => d.Id == userId);
        if (user is null)
        {
            throw new AppException("کاربر مورد نظر یافت نشد");
        }
        return user;
    }

}
