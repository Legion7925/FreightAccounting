using FreightAccounting.Core.Model.Common;

namespace FreightAccounting.Core.Model.Debtors;

public class DebtorsQueryParameters : QueryParameters
{
    public bool? Paid { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);

    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

    public string? SearchedName { get; set; }

    public string? PlateNumber { get; set; }
}
