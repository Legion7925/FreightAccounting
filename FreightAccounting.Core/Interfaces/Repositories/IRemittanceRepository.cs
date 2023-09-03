using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model.Remittances;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IRemittanceRepository
{
    Task AddRemittance(AddUpdateRemittanceModel remittanceModel);
    Task DeleteRemittance(int remittanceId);
    IEnumerable<RemittanceEntityReportModel> GetRemittanceByRettmianceNumber(string remittanceNumber);
    RemittanceReportModel GetRemittancesBetweenDates(RemittanceQueryParameter queryParameters);
    Task UpdateRemittance(int remittanceId, AddUpdateRemittanceModel remittanceModel);
    int GetRemittanceReportCount(DateTime startDate, DateTime endDate, int? operatorUserId);
}
