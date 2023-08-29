namespace FreightAccounting.WPF.Helper;

public static class AppSession
{
    public static AppSettings AppSettings { get; set; } = new();

    public static string LoggedInUsername { get; set; } = string.Empty;

    public static int LoggedInUserId { get; set; }
}

public class AppSettings
{
    public int OrganizationPercentage { get; set; } = 9;
    public int InsurancePercentage { get; set; } = 5;
    public int TaxPercentage { get; set; } = 1;

}
