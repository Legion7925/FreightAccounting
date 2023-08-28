using FreightAccounting.Core.Common;

namespace FreightAccounting.Core.Entities;

public class User : BaseEntity
{
    public required string NameAndFamily { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}
