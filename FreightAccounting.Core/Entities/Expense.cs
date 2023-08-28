using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class Expense : BaseEntity
{
    /// <summary>
    /// مخارج روز
    /// </summary>
    public int ExpensesAmount { get; set; }


    /// <summary>
    /// تاریخ روز
    /// </summary>
    public DateTime SubmitDate { get; set; }

    /// <summary>
    /// درآمد به دست آمده در روز
    /// </summary>
    public int Income { get; set; }
}
