using FreightAccounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FreightAccounting.Core;

public class FreightAccountingContext : DbContext
{
    public FreightAccountingContext()
    {
        
    }

    public FreightAccountingContext(DbContextOptions<FreightAccountingContext> options) 
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .Build();

        string connectionString = configuration.GetConnectionString("SqlConnectionString") ?? string.Empty;

        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Remittance> Remittances { get; set; }

    public DbSet<Debtor> Debtors { get; set; }

    public DbSet<OperatorUser> OperatorUsers { get; set; }

    public DbSet<Expense> Expenses { get; set; }
}
