using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Expenses;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IExpensesRepository
{
    Task AddExpense(AddUpdateExpenseModel expenseModel);
    Task DeleteExpense(int expenseId);
    Task<ExpensesReportModel> GetExpensesReport(ExpensesQueryParameters queryParameters);
    Task UpdateExpense(int expenseId, AddUpdateExpenseModel expenseModel);
    Task<int> GetExpenseReportCount(DateTime startDate, DateTime endDate);
}
