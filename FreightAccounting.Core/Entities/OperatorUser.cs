using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class OperatorUser : BaseEntity
{
    public required string Name { get; set; }

    public required string Family { get; set; }

    public ICollection<Remittance> Remittances { get; set; } = new List<Remittance>();
}
