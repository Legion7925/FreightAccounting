using FreightAccounting.Core;
using FreightAccounting.Core.Common;
using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
      .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
      .AddJsonFile("appsettings.json")
      .Build();

        string connectionString = configuration.GetConnectionString("SqlConnectionString") ?? string.Empty;


        services.AddScoped<IRemittanceRepository, RemittanceRepository>();
        services.AddScoped<IOperatorUserRepository, OperatorUserRepository>();
        services.AddScoped<IDebtorRepository, DebtorRepository>();
        services.AddScoped<IExpensesRepository, ExpensesRepository>();

        services.AddDbContext<FreightAccountingContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnectionString")));

        MigrateDatabase(services);  

        services.AddSingleton<MainWindow>();
    }
    private async void MigrateDatabase(ServiceCollection services)
    {
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var dbContext = serviceProvider.GetRequiredService<FreightAccountingContext>();
            dbContext.Database.Migrate();
            var rootUserExists = await dbContext.Users.AnyAsync(u => u.Username == "root");

            if (rootUserExists is not true)
            {
                await dbContext.Users.AddAsync(new User { NameAndFamily = "root", Password = PasswordHasher.HashPassword("123qwe!@#"), Username = "root" });
                await dbContext.SaveChangesAsync();
            }

        }

    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow!.Show();
    }
}
