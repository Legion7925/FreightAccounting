using FreightAccounting.Core;
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

        services.AddDbContext<FreightAccountingContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnectionString")));


        services.AddSingleton<MainWindow>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow!.Show();
    }
}
