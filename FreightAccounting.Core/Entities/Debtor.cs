using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class Debtor : BaseEntity
{
    public required string DriverFirstName { get; set; }

    public required string DriverLastName { get; set; }

    /// <summary>
    /// مقصد
    /// </summary>
    public required string Destination { get; set; }

    /// <summary>
    /// شماره پلاک
    /// </summary>
    public required string PlateNumber { get; set; }

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// مقدار بدهی
    /// </summary>
    public int DebtAmount { get; set; }

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
