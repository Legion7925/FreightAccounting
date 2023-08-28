namespace FreightAccounting.Core.Model;

public class AddUpdateExpenseModel
{
    /// <summary>
    /// مخارج روز
    /// </summary>
    public int ExpensesAmount { get; set; }

    /// <summary>
    /// تاریخ روز
    /// </summary>
    public DateTime SubmitDate { get; set; }
}
