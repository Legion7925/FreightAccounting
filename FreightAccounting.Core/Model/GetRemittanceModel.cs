using FreightAccounting.Core.Entities;

namespace FreightAccounting.Core.Model;

public class GetRemittanceModel
{
    public List<Remittance> Remittances { get; set; } = new();

    //درآمد بین دو تاریخ
    public int SumIncome => Remittances.Select(r => r.ReceviedCommission).Sum();

    //سود خالص بین دو تاریخ
    public int SumNetProfit => Remittances.Select(r=> r.NetProfit).Sum();

    //مالیات پرداختی بین دو تاریخ
    public int SumTaxPayment => Remittances.Select(r => r.TaxPayment).Sum();

    //جمع کارکرد کاربر بین دو تاریخ
    public int SumUserCut => Remittances.Select(r=> r.UserCut).Sum();

    //جمع پرداختی به بیمه
    public int SumInsurancePayment => Remittances.Select(r=> r.InsurancePayment).Sum();
}
