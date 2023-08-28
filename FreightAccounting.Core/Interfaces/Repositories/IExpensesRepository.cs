using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Expenses;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IExpensesRepository
{
    Task AddExpense(AddUpdateExpenseModel expenseModel);
    Task DeleteExpense(int expenseId);
    Task<IEnumerable<Expense>> GetExpensesReport(QueryParameters queryParameters);
    Task UpdateExpense(int expenseId, AddUpdateExpenseModel expenseModel);
}
