namespace FreightAccounting.Core.Model.Remittances;

public class AddUpdateRemittanceModel
{
    /// <summary>
    /// شماره بارنامه
    /// </summary>
    public string RemittanceNumber { get; set; } = string.Empty;    

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
    /// مالیات دارایی
    /// </summary>
    public long TaxPayment { get; set; }

    /// <summary>
    /// آیدی کاربر ثبت کننده
    /// </summary>
    public int OperatorUserId { get; set; }

    /// <summary>
    /// کمیسیون دریافتی
    /// </summary>
    public long ReceviedCommission { get; set; }

    /// <summary>
    /// پورسانت کاربر
    /// </summary>
    public long UserCut { get; set; }

    /// <summary>
    /// تاریخ ثبت بارنامه
    /// </summary>
    public DateTime SubmitDate { get; set; }

    /// <summary>
    /// شماره بیمه کالا
    /// </summary>
    public long ProductInsurancePayment { get; set; }



}
