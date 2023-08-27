namespace FreightAccounting.Core.Model;

public class RemittanceQueryParameter : QueryParameters
{
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);

    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

    public int? OperatorUserId { get; set; }

}
