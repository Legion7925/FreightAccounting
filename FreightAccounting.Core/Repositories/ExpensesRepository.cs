﻿using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core.Repositories;

public class ExpensesRepository : IExpensesRepository
{
    private readonly FreightAccountingContext _context;

    public ExpensesRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Expense>> GetExpensesReport(QueryParameters queryParameters)
    {
        return await _context.Expenses
           .AsNoTracking()
           .Skip((queryParameters.Page - 1) * queryParameters.Size)
           .Take(queryParameters.Size)
           .ToArrayAsync();
    }

    public async Task AddExpense(AddUpdateExpenseModel expenseModel)
    {
        //سود خالص روزی که وارد کرده رو از دیتا بیس میکشیم بیرون همرو با هم جمع میزنیم
        var submittedDateNetProfit = _context.Remittances.Where(r => r.SubmitDate.Date == expenseModel.SubmitDate.Date).Select(r => r.NetProfit).Sum();

        var expense = new Expense
        {
            ExpensesAmount = expenseModel.ExpensesAmount,
            SubmitDate = expenseModel.SubmitDate,
            Income = submittedDateNetProfit - expenseModel.ExpensesAmount
        };

        await _context.Expenses.AddAsync(expense);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateExpense(int expenseId, AddUpdateExpenseModel expenseModel)
    {
        var expense = await GetExpenseById(expenseId);

        expense.ExpensesAmount = expenseModel.ExpensesAmount;
        expense.SubmitDate = expenseModel.SubmitDate;

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