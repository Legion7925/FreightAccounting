using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Model.Expenses;

public class ExpensesReportModel
{
    /// <summary>
    /// لیست مخارج
    /// </summary>
    public IEnumerable<ExpenseEntityReportModel> Expenses { get; set; } = new List<ExpenseEntityReportModel>();

    /// <summary>
    /// مجموع مخارج بین دو تاریخ
    /// </summary>
    public long TotalExpensesAmount { get; set; } /*=> Expenses.Select(e => e.ExpensesAmount).Sum();*/

    /// <summary>
    /// مجموع درآمد خالص بین دو تاریخ
    /// </summary>
    public long TotalIncome { get; set; } /*=> Expenses.Select(e => e.Income).Sum();*/
}
