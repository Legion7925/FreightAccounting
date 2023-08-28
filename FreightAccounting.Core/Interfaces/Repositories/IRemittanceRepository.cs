using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model.Remittances;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IRemittanceRepository
{
    Task AddRemittance(AddUpdateRemittanceModel remittanceModel);
    Task DeleteRemittance(int remittanceId);
    Task<Remittance> GetRemittanceByRettmianceNumber(string remittanceNumber);
    Task<RemittanceReportModel> GetRemittancesBetweenDates(RemittanceQueryParameter queryParameters);
    Task UpdateRemittance(int remittanceId, AddUpdateRemittanceModel remittanceModel);
}
