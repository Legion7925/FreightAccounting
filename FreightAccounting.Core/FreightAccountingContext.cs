using FreightAccounting.Core.Common;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Remittance>().HasIndex(r => r.RemittanceNumber);

        modelBuilder.Entity<Debtor>().HasIndex(d => d.DriverFirstName);

        modelBuilder.Entity<Debtor>().HasIndex(d => d.DriverLastName);

        modelBuilder.Entity<User>().HasData
        (
            new User {Id=1, NameAndFamily = "root", Username = "root", Password = PasswordHasher.HashPassword("123qwe!@#") },
            new User {Id=2, NameAndFamily = "kaveh", Username = "kaveh", Password = PasswordHasher.HashPassword("@2205") }
        );
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Remittance> Remittances { get; set; }

    public DbSet<Debtor> Debtors { get; set; }

    public DbSet<OperatorUser> OperatorUsers { get; set; }

    public DbSet<Expense> Expenses { get; set; }

    public DbSet<User> Users { get; set; }
}
