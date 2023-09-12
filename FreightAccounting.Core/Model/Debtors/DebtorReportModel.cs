namespace FreightAccounting.Core.Model.Debtors;

public class DebtorReportModel
{
    public List<DebtorEntityReportModel> DebtorsList { get; set; } = new List<DebtorEntityReportModel>();

    public long TotalDebt { get; set; }
}
