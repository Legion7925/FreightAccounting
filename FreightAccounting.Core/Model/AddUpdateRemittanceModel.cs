namespace FreightAccounting.Core.Model;

public class AddUpdateRemittanceModel
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
    public required string SubmittedUsername { get; set; }

    /// <summary>
    /// کمیسیون دریافتی
    /// </summary>
    public int ReceviedCommission { get; set; }

    /// <summary>
    /// پورسانت کاربر
    /// </summary>
    public int UserCut { get; set; }

    /// <summary>
    /// تاریخ ثبت بارنامه
    /// </summary>
    public DateTime SubmitDate { get; set; }


}
