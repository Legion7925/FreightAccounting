using FreightAccounting.Core.Entities;
using PersianDate.Standard;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreightAccounting.Core.Model.Remittances
{
    public class RemittanceEntityReportModel
    {
        public int Id { get; set; }

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
        public long ProductInsurancePayment { get; set; }

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


        public string SubmitDateFa => SubmitDate.ToFa();
    }
}
