using FreightAccounting.Core;
using FreightAccounting.Core.Common;
using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Repositories;
using FreightAccounting.WPF.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stimulsoft.Report;
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
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDbContext<FreightAccountingContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnectionString")));

        MigrateDatabase(services);  

        services.AddSingleton<LoginWindow>();

        Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkgpgFGkUl79uxVs8X+uspx6K+tqdtOB5G1S6PFPRrlVNvMUiSiNYl724EZbrUAWwAYHlGL" +
                "RbvxMviMExTh2l9xZJ2xc4K1z3ZVudRpQpuDdFq+fe0wKXSKlB6okl0hUd2ikQHfyzsAN8fJltqvGRa5LI8BFkA/f7tffwK6jzW5xYYhHxQpU3hy4fmKo/BSg6yKAoUq3yMZTG6tWeKnWc" +
                "I6ftCDxEHd30EjMISNn1LCdLN0/4YmedTjM7x+0dMiI2Qif/yI+y8gmdbostOE8S2ZjrpKsgxVv2AAZPdzHEkzYSzx81RHDzZBhKRZc5mwWAmXsWBFRQol9PdSQ8BZYLqvJ4Jzrcrext+t1" +
                "ZD7HE1RZPLPAqErO9eo+7Zn9Cvu5O73+b9dxhE2sRyAv9Tl1lV2WqMezWRsO55Q3LntawkPq0HvBkd9f8uVuq9zk7VKegetCDLb0wszBAs1mjWzN+ACVHiPVKIk94/QlCkj31dWCg8YTrT" +
                "5btsKcLibxog7pv1+2e4yocZKWsposmcJbgG0";

        //فارسی سازی گزارشات استیمال چه در حالت طراحی چه نمایش
        StiOptions.Localization.Load(@"Resources\StimulsoftLanguages\fa.xml");
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var loginWindow = serviceProvider.GetService<LoginWindow>();
        loginWindow!.Show();
    }

    private async void MigrateDatabase(ServiceCollection services)
    {
        try
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<FreightAccountingContext>();
                dbContext.Database.Migrate();
                var rootUserExists = await dbContext.Users.AnyAsync(u => u.Username == "root");

                if (rootUserExists is not true)
                {
                    await dbContext.Users.AddAsync(new User { NameAndFamily = "kaveh", Password = PasswordHasher.HashPassword("@2205"), Username = "kaveh" });
                    await dbContext.Users.AddAsync(new User { NameAndFamily = "root", Password = PasswordHasher.HashPassword("123qwe!@#"), Username = "root" });
                    await dbContext.SaveChangesAsync();
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("خطا در ارتباط با پایگاه داده");
            Logger.LogException(ex);
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
