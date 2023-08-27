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
    public int TransforPayment { get; set; }

    /// <summary>
    /// هزینه سازمان
    /// </summary>
    public int OrganizationPayment { get; set; }

    /// <summary>
    /// هزینه بیمه
    /// </summary>
    public int InsurancePayment { get; set; }

    /// <summary>
    /// مالیات دارایی
    /// </summary>
    public int TaxPayment { get; set; }

    /// <summary>
    /// نام کاربر ثبت کننده
    /// </summary>
    public string? SubmittedUsername { get; set; }

    [ForeignKey("OperatorUser")]
    public int OperatorUserId { get; set; }

    /// <summary>
    /// پورسانت کاربر
    /// </summary>
    public int UserCut { get; set; }

    /// <summary>
    /// سود خالص
    /// </summary>
    public int NetProfit { get; set; }

    /// <summary>
    /// کمیسیون دریافتی
    /// </summary>
    public int ReceviedCommission { get; set; }

    /// <summary>
    /// تاریخ ثبت بارنامه
    /// </summary>
    public DateTime SubmitDate { get; set; }

    public OperatorUser? OperatorUser { get; set; }
}
