using FreightAccounting.Core.Model.Common;

namespace FreightAccounting.Core.Model.Debtors;

public class DebtorsQueryParameters : QueryParameters
{
    public bool? Paid { get; set; }
}
