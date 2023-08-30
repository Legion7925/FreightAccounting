using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Model.Remittances;

public class RemittanceReportModel
{
    public IEnumerable<RemittanceEntityReportModel> Remittances { get; set; } = new List<RemittanceEntityReportModel>();

    //درآمد بین دو تاریخ
    public int SumIncome { get; set; } 

    //سود خالص بین دو تاریخ
    public int SumNetProfit { get; set; } 

    //مالیات پرداختی بین دو تاریخ
    public int SumTaxPayment { get; set; } 

    //جمع پرداختی به بیمه
    public int SumInsurancePayment { get; set; } 

    //جمع پورسانت کاربر یا کاربران
    public int SumUserCut { get; set; } 

}
