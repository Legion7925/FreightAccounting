namespace FreightAccounting.Core.Model;

public class AddUpdateDebtorModel
{
    public required string DriverName { get; set; }

    public required string DriverFamilyName { get; set; }

    /// <summary>
    /// مقصد
    /// </summary>
    public required string Destination { get; set; }

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// مقدار بدهی
    /// </summary>
    public int DebtAmount { get; set; }
}
