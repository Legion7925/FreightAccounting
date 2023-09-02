using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Model.Remittances;

public class RemittanceReportModel
{
    public IEnumerable<RemittanceEntityReportModel> Remittances { get; set; } = new List<RemittanceEntityReportModel>();

    //درآمد بین دو تاریخ
    public long SumIncome { get; set; } 

    //سود خالص بین دو تاریخ
    public long SumNetProfit { get; set; } 

    //مالیات پرداختی بین دو تاریخ
    public long SumTaxPayment { get; set; } 

    //جمع پرداختی به بیمه
    public long SumInsurancePayment { get; set; } 

    //جمع پورسانت کاربر یا کاربران
    public long SumUserCut { get; set; } 

}
