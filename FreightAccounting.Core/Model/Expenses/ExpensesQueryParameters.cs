using FreightAccounting.Core.Model.Common;

namespace FreightAccounting.Core.Model.Expenses;

public class ExpensesQueryParameters : QueryParameters
{
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);

    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

}
