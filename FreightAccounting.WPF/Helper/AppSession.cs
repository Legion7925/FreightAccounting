
namespace FreightAccounting.WPF.Helper;

public static class AppSession
{
    public static AppSettings AppSettings { get; set; } = new();

    public static string LoggedInUsername { get; set; } = string.Empty;

    public static int LoggedInUserId { get; set; }

    public static Mohsen.PersianDate LastSubmittedDate { get; set; } = Mohsen.PersianDate.Today;

}

public class AppSettings
{
    public double OrganizationPercentage { get; set; } = 9;
    public double InsurancePercentage { get; set; } = 5;
    public double TaxPercentage { get; set; } = 1;
    public double UserCutPercentage { get; set; } = 4;
}
