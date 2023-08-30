using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IOperatorUserRepository
{
    IEnumerable<OperatorUser> GetOperatorUsers();
    Task AddOperatorUser(OperatorUser operatorUser);
    Task DeleteOperatorUser(int userId);
    Task UpdateOperatorUser(int userId, OperatorUser operatorUser);
}
