using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Model.Remittances;

public class RemittanceReportModel
{
    public List<Remittance> Remittances { get; set; } = new();

    //درآمد بین دو تاریخ
    public int SumIncome { get; set; } 

    //سود خالص بین دو تاریخ
    public int SumNetProfit { get; set; } 

    //مالیات پرداختی بین دو تاریخ
    public int SumTaxPayment { get; set; } 

    //جمع پرداختی به بیمه
    public int SumInsurancePayment { get; set; } 

    //جمع کارکرد کاربر بین دو تاریخ این مقدار فقط وقتی برای نمایش مناسبه که گزارش بر اساس آیدی یک کاربر باشه
    public int SumUserCut { get; set; } 

}
