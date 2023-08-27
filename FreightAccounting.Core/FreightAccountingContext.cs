using FreightAccounting.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core;

public class FreightAccountingContext : DbContext
{
    public FreightAccountingContext(DbContextOptions<FreightAccountingContext> options) 
        : base(options)
    {
        
    }

    public DbSet<Remittance> Remittances { get; set; }

    public DbSet<Debtor> Debtors { get; set; }
}
