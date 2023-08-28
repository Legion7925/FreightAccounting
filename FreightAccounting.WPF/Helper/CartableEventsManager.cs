using System;

namespace FreightAccounting.WPF.Helper;

public class CartableEventsManager
{
    public static event EventHandler? updateDebtorDatagrid;
    public static event EventHandler? updateRemittanceDatagrid;

    public static void OnUpdateDebtorDatagrid()
    {
        updateDebtorDatagrid?.Invoke(default!, EventArgs.Empty);
    }

    public static void OnUpdateRemittanceDatagrid()
    {
        updateRemittanceDatagrid?.Invoke(default!, EventArgs.Empty);
    }
}
