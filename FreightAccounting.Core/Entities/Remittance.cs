using FreightAccounting.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreightAccounting.Core.Entities;

public class Remittance : BaseEntity
{
    /// <summary>
    /// شماره بارنامه
    /// </summary>
    public required string RemittanceNumber { get; set; }

    /// <summary>
    /// کرایه راننده
    /// </summary>
    public long TransforPayment { get; set; }

    /// <summary>
    /// هزینه سازمان
    /// </summary>
    public long OrganizationPayment { get; set; }

    /// <summary>
    /// هزینه بیمه
    /// </summary>
    public long InsurancePayment { get; set; }

    /// <summary>
    /// شماره بیمه کالا
    /// </summary>
    public long ProductInsuranceNumber { get; set; }

    /// <summary>
    /// مالیات دارایی
    /// </summary>
    public long TaxPayment { get; set; }

    /// <summary>
    /// نام کاربر ثبت کننده
    /// </summary>
    public string? SubmittedUsername { get; set; }

    [ForeignKey("OperatorUser")]
    public int OperatorUserId { get; set; }

    /// <summary>
    /// پورسانت کاربر
    /// </summary>
    public long UserCut { get; set; }

    /// <summary>
    /// سود خالص
    /// </summary>
    public long NetProfit { get; set; }

    /// <summary>
    /// کمیسیون دریافتی
    /// </summary>
    public long ReceviedCommission { get; set; }

    /// <summary>
    /// تاریخ ثبت بارنامه
    /// </summary>
    public DateTime SubmitDate { get; set; }

    public OperatorUser? OperatorUser { get; set; }
}
