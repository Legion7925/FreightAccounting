﻿namespace FreightAccounting.Core.Model.Debtors;

public class AddUpdateDebtorModel
{
    public required string DriverFirstName { get; set; }

    public required string DriverLastName { get; set; }

    public required string PlateNumber { get; set; }

    /// <summary>
    /// مقصد
    /// </summary>
    public required string Destination { get; set; }

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// مقدار بدهی
    /// </summary>
    public long DebtAmount { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// تاریخ ثبت
    /// </summary>
    public DateTime SubmitDate { get; set; }
}
