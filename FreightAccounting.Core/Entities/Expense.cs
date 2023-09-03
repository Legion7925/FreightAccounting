using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class Expense : BaseEntity
{
    /// <summary>
    /// مخارج روز
    /// </summary>
    public long ExpensesAmount { get; set; }


    /// <summary>
    /// تاریخ روز
    /// </summary>
    public DateTime SubmitDate { get; set; }

    /// <summary>
    /// درآمد به دست آمده در روز
    /// </summary>
    public long Income { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; set; } = string.Empty;

}
