﻿namespace FreightAccounting.Core.Model.Expenses;

public class AddUpdateExpenseModel
{
    /// <summary>
    /// مخارج روز
    /// </summary>
    public long ExpensesAmount { get; set; }

    /// <summary>
    /// تاریخ روز
    /// </summary>
    public DateTime SubmitDate { get; set; }
}
