using System;
using System.Windows;

namespace FreightAccounting.WPF.Helper;

public class CartableEventsManager
{
    public static event EventHandler? updateDebtorDatagrid;
    public static event EventHandler<RoutedEventArgs>? updateRemittanceDatagrid;
    public static event EventHandler<RoutedEventArgs>? updateExpensesDatagrid;
    public static event EventHandler? updateOperatorUserCombobox;

    public static void OnUpdateDebtorDatagrid()
    {
        updateDebtorDatagrid?.Invoke(default!, EventArgs.Empty);
    }

    public static void OnUpdateRemittanceDatagrid()
    {
        updateRemittanceDatagrid?.Invoke(default!, default!);
    }

    public static void OnUpdateExpensesDatagrid()
    {
        updateExpensesDatagrid?.Invoke(default!, default!);
    }

    public static void OnUpdateOperatorUserCombobox()
    {
        updateOperatorUserCombobox?.Invoke(default!, EventArgs.Empty);
    }
}
