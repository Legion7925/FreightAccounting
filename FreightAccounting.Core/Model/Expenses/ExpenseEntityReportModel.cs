using FreightAccounting.Core.Entities;
using PersianDate.Standard;

namespace FreightAccounting.Core.Model.Expenses;

public class ExpenseEntityReportModel : Expense
{
    public string SubmitDateFa => SubmitDate.ToFa();
}
