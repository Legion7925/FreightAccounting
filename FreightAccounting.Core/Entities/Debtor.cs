using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class Debtor : BaseEntity
{
    public  string DriverFirstName { get; set; } = string .Empty;

    public string DriverLastName { get; set; } = string.Empty;

    /// <summary>
    /// مقصد
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// شماره پلاک
    /// </summary>
    public string PlateNumber { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// مقدار بدهی
    /// </summary>
    public long DebtAmount { get; set; }

    /// <summary>
    /// تاریخ پرداخت میتونه نال باشه
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// پرداخت کرده یا نه اگر تاریخ پرداخت نال باشه این فالس میمونه
    /// اگر نال نباشه تورو میشه خود به خود
    /// </summary>
    public bool Paid => PaymentDate != null;
}
