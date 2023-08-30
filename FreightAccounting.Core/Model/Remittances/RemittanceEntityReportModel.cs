using FreightAccounting.Core.Entities;
using PersianDate.Standard;

namespace FreightAccounting.Core.Model.Remittances
{
    public class RemittanceEntityReportModel : Remittance
    {
        public string SubmitDateFa => SubmitDate.ToFa();
    }
}
