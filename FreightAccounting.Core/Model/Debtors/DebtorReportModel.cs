namespace FreightAccounting.Core.Model.Debtors;

public class DebtorReportModel
{
    public IEnumerable<DebtorEntityReportModel> DebtorsList { get; set; } = new List<DebtorEntityReportModel>();

    public long TotalDebt { get; set; }
}
