using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IDebtorRepository
{
    Task AddDebtor(AddUpdateDebtorModel debtorModel);
    Task DeleteDebtor(int debtorId);
    Task<IEnumerable<Debtor>> GetDebtors(QueryParameters queryParameters);
    Task SubmitPayment(int debtorId);
    Task UpdateDebtor(int debtorId, AddUpdateDebtorModel debtorModel);
}
