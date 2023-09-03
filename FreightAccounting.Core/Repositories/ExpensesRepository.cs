using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Expenses;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core.Repositories;

public class ExpensesRepository : IExpensesRepository
{
    private readonly FreightAccountingContext _context;

    public ExpensesRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    public int GetExpenseReportCount(DateTime startDate , DateTime endDate)
    {
        return _context.Expenses
           .AsNoTracking()
           .Where(e => e.SubmitDate.Date >= startDate.Date && e.SubmitDate <= endDate.Date).Count();
    }

    public ExpensesReportModel GetExpensesReport(ExpensesQueryParameters queryParameters)
    {
        var expenses = _context.Expenses
           .AsNoTracking()
           .Where(e => e.SubmitDate.Date >= queryParameters.StartDate.Date && e.SubmitDate <= queryParameters.EndDate.Date);

        var paginatedExpenses = expenses
           .OrderByDescending(e => e.SubmitDate)
           .Skip((queryParameters.Page - 1) * queryParameters.Size)
           .Take(queryParameters.Size)
           .Select(e => new ExpenseEntityReportModel
           {
               Id = e.Id,
               SubmitDate = e.SubmitDate,
               ExpensesAmount = e.ExpensesAmount,
               Income = e.Income,
               Description = e.Description
           });


        return new ExpensesReportModel
        {
            Expenses = paginatedExpenses.ToList(),
            TotalExpensesAmount = expenses.Select(e => e.ExpensesAmount).Sum(),
            TotalIncome = expenses.Select(e=> e.Income).Sum(),
        };
    }

    public async Task AddExpense(AddUpdateExpenseModel expenseModel)
    {
        var expenseExist = _context.Expenses.Where(e=> e.SubmitDate == expenseModel.SubmitDate).Any();
        if(expenseExist)
        {
            throw new AppException("مخارج این تاریخ قبلا ثبت شده است");
        }

        //سود خالص روزی که وارد کرده رو از دیتا بیس میکشیم بیرون همرو با هم جمع میزنیم
        var submittedDateNetProfit = _context.Remittances.Where(r => r.SubmitDate.Date == expenseModel.SubmitDate.Date).Select(r => r.NetProfit).Sum();

        var expense = new Expense
        {
            ExpensesAmount = expenseModel.ExpensesAmount,
            SubmitDate = expenseModel.SubmitDate,
            Income = submittedDateNetProfit - expenseModel.ExpensesAmount,
            Description = expenseModel.Description
        };

        await _context.Expenses.AddAsync(expense);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateExpense(int expenseId, AddUpdateExpenseModel expenseModel)
    {
        var expense = await GetExpenseById(expenseId);
        //سود خالص روزی که وارد کرده رو از دیتا بیس میکشیم بیرون همرو با هم جمع میزنیم
        var submittedDateNetProfit = _context.Remittances.Where(r => r.SubmitDate.Date == expenseModel.SubmitDate.Date).Select(r => r.NetProfit).Sum();

        expense.ExpensesAmount = expenseModel.ExpensesAmount;
        expense.SubmitDate = expenseModel.SubmitDate;
        expense.Income = submittedDateNetProfit - expenseModel.ExpensesAmount;
        expense.Description = expenseModel.Description;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpense(int expenseId)
    {
        var expense = await GetExpenseById(expenseId);

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
    }

    private async Task<Expense> GetExpenseById(int expenseId)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(d => d.Id == expenseId);
        if (expense is null)
        {
            throw new AppException("کاربر مورد نظر یافت نشد");
        }
        return expense;
    }
}
