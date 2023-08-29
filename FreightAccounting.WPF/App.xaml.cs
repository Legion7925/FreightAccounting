using FreightAccounting.Core;
using FreightAccounting.Core.Common;
using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Repositories;
using FreightAccounting.WPF.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Runtime;
using System.Text.Json;
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
        ReadSettingsFromJsonFile();
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

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow!.Show();
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

    /// <summary>
    /// تنظیمات نرم افزار از یک فایل جیسون از مسیر روت نرم افزار خوانده میشود
    /// و در نهایت بر روی برنامه اعمال میشود
    /// </summary>
    private void ReadSettingsFromJsonFile()
    {
        try
        {
            var path = $@"{Environment.CurrentDirectory}\Setting.json";
            if (!File.Exists(path))
                return;
            var JsonString = File.ReadAllText(path);
            var setting = JsonSerializer.Deserialize<AppSettings>(JsonString);
            if (setting != null)
                AppSession.AppSettings = setting;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

}
