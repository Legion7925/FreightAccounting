using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Debtors;
using FreightAccounting.Core.Model.Expenses;
using FreightAccounting.Core.Model.Remittances;
using FreightAccounting.Core.Repositories;
using FreightAccounting.WPF.Helper;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDebtorRepository _debtorRepository;
    private readonly IRemittanceRepository _remittanceRepository;
    private readonly IOperatorUserRepository _operatorUserRepository;
    private readonly IExpensesRepository _expensesRepository;
    private IEnumerable<DebtorReportModel> debtorsList = new List<DebtorReportModel>();
    private ExpensesReportModel expensesReportModel = new ExpensesReportModel();
    private RemittanceReportModel remittanceReportModel = new RemittanceReportModel();
    private DebtorReportModel selectedDebtor = new DebtorReportModel();
    //private RemittanceReportModel selectedRemitance = new RemittanceReportModel();
    private Remittance selectedRemittance = new Remittance() { RemittanceNumber = string.Empty };
    private ExpenseEntityReportModel selectedExpense = new ExpenseEntityReportModel();
    private int _selectedId;

    public MainWindow(IDebtorRepository debtorRepository,
        IRemittanceRepository remittanceRepository,
        IOperatorUserRepository operatorUserRepository,
        IExpensesRepository expensesRepository)
    {
        InitializeComponent();
        _debtorRepository = debtorRepository;
        _remittanceRepository = remittanceRepository;
        _operatorUserRepository = operatorUserRepository;

        NotificationEventsManager.showMessage += ShowSnackbarMessage;
        CartableEventsManager.updateDebtorDatagrid += FillDebtorDatagrid;
        CartableEventsManager.updateRemittanceDatagrid += FillRemitanceDatagrid;
        CartableEventsManager.updateExpensesDatagrid += FillExpensesDatagrid;
        _expensesRepository = expensesRepository;

        dpExpensesReportStart.SelectedDate = Mohsen.PersianDate.Today.AddDays(-2);
        dpExpensesReportEnd.SelectedDate = Mohsen.PersianDate.Today;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        FillDebtorDatagrid(null, null);
        FillExpensesDatagrid(null, null);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void gridHears_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void btnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void btnMaximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
        else
            WindowState = WindowState.Normal;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void btnChangeTheme_Checked(object sender, RoutedEventArgs e)
    {
        var paletteHelper = new PaletteHelper();
        //Retrieve the app's existing theme
        ITheme theme = paletteHelper.GetTheme();

        //Change the base theme to Dark
        theme.SetBaseTheme(Theme.Dark);
        //or theme.SetBaseTheme(Theme.Light);

        //Change all of the primary colors to Red
        theme.SetPrimaryColor(Colors.Blue);

        //Change all of the secondary colors to Blue
        theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue]);

        ////You can also change a single color on the theme, and optionally set the corresponding foreground color
        //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

        //Change the app's current theme
        paletteHelper.SetTheme(theme);

        //todo -- save to app setting for when user open after change theme -- todo
    }

    private void btnChangeTheme_Unchecked(object sender, RoutedEventArgs e)
    {
        var paletteHelper = new PaletteHelper();
        //Retrieve the app's existing theme
        ITheme theme = paletteHelper.GetTheme();

        //Change the base theme to Dark
        theme.SetBaseTheme(Theme.Light);
        //or theme.SetBaseTheme(Theme.Light);

        //Change all of the primary colors to Red
        theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue]);

        ////Change all of the secondary colors to Blue
        //theme.SetSecondaryColor(Colors.Blue);

        ////You can also change a single color on the theme, and optionally set the corresponding foreground color
        //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

        //Change the app's current theme
        paletteHelper.SetTheme(theme);

        //todo -- save to app setting for when user open after change theme -- todo
    }

    private void btnChangePassword_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnAddRemittance_Click(object sender, RoutedEventArgs e)
    {
        new AddRemitance(_remittanceRepository, _operatorUserRepository, false, null, null).ShowDialog();
    }


    /// <summary>
    /// نمایش پیام سیستم
    /// </summary>
    /// <param name="messageType">نوع پیام</param>
    private void ShowSnackbarMessage(object? sender, MessageTypeEnum messageType)
    {
        if (sender == null)
            return;

        switch (messageType)
        {
            case MessageTypeEnum.Success:
                snackBarSuccess.MessageQueue?.Enqueue((string)sender, null, null, null, false, true, TimeSpan.FromSeconds(3));
                break;
            case MessageTypeEnum.Error:
                snackBarError.MessageQueue?.Enqueue((string)sender, null, null, null, false, true, TimeSpan.FromSeconds(3));
                break;
            case MessageTypeEnum.Warning:
                snackBarWarning.MessageQueue?.Enqueue((string)sender, null, null, null, false, true, TimeSpan.FromSeconds(3));
                break;
            case MessageTypeEnum.Information:
                snackBarInfo.MessageQueue?.Enqueue((string)sender, null, null, null, false, true, TimeSpan.FromSeconds(3));
                break;
            default:
                snackBarInfo.MessageQueue?.Enqueue((string)sender, null, null, null, false, true, TimeSpan.FromSeconds(3));
                break;
        }
    }

    private async void FillRemitanceDatagrid(object? sender, EventArgs? e)
    {
        try
        {
            remittanceReportModel = await _remittanceRepository.GetRemittancesBetweenDates(
            new RemittanceQueryParameter
            {
                StartDate = dpRemittanceStart.DisplayDate.ToDateTime(),
                EndDate = dpRemittanceEnd.DisplayDate.ToDateTime(),
            });

            dgReport.ItemsSource = remittanceReportModel.Remittances;
        }
        catch (AppException ax)
        {
            ShowSnackbarMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage(ex.Message, MessageTypeEnum.Error);
        }
    }

    private void dgReport_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dgReport.SelectedItems.Count > 0)
            selectedRemittance = dgReport.SelectedItem as Remittance ?? new Remittance() { RemittanceNumber = string.Empty };
    }

    #region Debtors
    private void btnAddDebtors_Click(object sender, RoutedEventArgs e)
    {
        var addDebtorWindow = new AddDebtorWindow(_debtorRepository, false, null, null);
        addDebtorWindow.ShowDialog();
    }

    /// <summary>
    /// پر کردن دیتاگرید بدهکاران
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void FillDebtorDatagrid(object? sender, EventArgs? e)
    {
        try
        {
            debtorsList = await _debtorRepository.GetDebtors(new QueryParameters());
            dgDebtorsReport.ItemsSource = debtorsList;
        }
        catch (AppException ax)
        {
            ShowSnackbarMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage(ex.Message, MessageTypeEnum.Error);
        }
    }

    private void dgDebtorsReport_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dgDebtorsReport.SelectedItems.Count > 0)
            selectedDebtor = dgDebtorsReport.SelectedItem as DebtorReportModel ?? new DebtorReportModel();
    }

    /// <summary>
    /// رخداد تغییر کومبوباکس وضعیت پرداخت بدهکاران
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbFilterPaymentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgDebtorsReport is null)
            return;

        switch (cmbFilterPaymentStatus.SelectedIndex)
        {
            case 0:
                dgDebtorsReport.ItemsSource = debtorsList;
                break;
            case 1:
                dgDebtorsReport.ItemsSource = debtorsList.Where(d => d.Paid == true).ToList();
                break;
            case 2:
                dgDebtorsReport.ItemsSource = debtorsList.Where(d => d.Paid == false).ToList();
                break;
        }
    }
    /// <summary>
    /// رخداد تغییر متن جست و جو بر اساس نام
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtSearchDebtorsByName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtSearchDebtorsByName.Text))
        {
            dgDebtorsReport.ItemsSource = debtorsList;
            return;
        }
        dgDebtorsReport.ItemsSource = debtorsList.Where(d => (d.DriverFirstName + d.DriverLastName).Contains(txtSearchDebtorsByName.Text)).ToList();
    }
    /// <summary>
    /// باز کردن پنجره ویرایش بدهکار
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnEditDebtor_Click(object sender, RoutedEventArgs e)
    {
        new AddDebtorWindow(_debtorRepository, true, selectedDebtor.Id, new AddUpdateDebtorModel
        {
            DebtAmount = selectedDebtor.DebtAmount,
            Destination = selectedDebtor.Destination,
            DriverFirstName = selectedDebtor.DriverFirstName,
            DriverLastName = selectedDebtor.DriverLastName,
            PlateNumber = selectedDebtor.PlateNumber,
            PhoneNumber = selectedDebtor.PhoneNumber
        }).ShowDialog();
    }
    /// <summary>
    /// حذف بدهکار
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDeleteDebtor_Click(object sender, RoutedEventArgs e)
    {
        new DeleteDebtorWindow(_debtorRepository, selectedDebtor.Id, true, _remittanceRepository, 0).ShowDialog();
    }

    private void btnSubmitPayment_Click(object sender, RoutedEventArgs e)
    {
        new SubmitDebtWindow(_debtorRepository, selectedDebtor.Id).ShowDialog();
    }

    #endregion Debtors

    #region Expenses
    private async void FillExpensesDatagrid(object? sender, EventArgs? e)
    {
        try
        {
            expensesReportModel = await _expensesRepository.GetExpensesReport(
            new ExpensesQueryParameters
            {
                StartDate = dpExpensesReportStart.DisplayDate.ToDateTime(),
                EndDate = dpExpensesReportEnd.DisplayDate.ToDateTime()
            });

            dgExpenses.ItemsSource = expensesReportModel.Expenses;
        }
        catch (AppException ax)
        {
            ShowSnackbarMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage(ex.Message, MessageTypeEnum.Error);
        }
    }

    private void btnAddExpense_Click(object sender, RoutedEventArgs e)
    {
        new AddExpenseWindow(_expensesRepository, false, null, null).ShowDialog();
    }

    private void btnEditExpense_Click(object sender, RoutedEventArgs e)
    {
        new AddExpenseWindow(_expensesRepository, true, selectedExpense.Id, new AddUpdateExpenseModel
        {
            ExpensesAmount = selectedExpense.ExpensesAmount,
            SubmitDate = selectedExpense.SubmitDate,
        }).ShowDialog();
    }

    private void dgExpenses_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dgExpenses.SelectedItems.Count > 0)
            selectedExpense = dgExpenses.SelectedItem as ExpenseEntityReportModel ?? new ExpenseEntityReportModel();
    }

    private void btnDeleteExpense_Click(object sender, RoutedEventArgs e)
    {
        new DeleteExpenseWindow(_expensesRepository, selectedExpense.Id).ShowDialog();
    }

    #endregion Expenses

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
    }

    private void btnDeleteRemitance_Click(object sender, RoutedEventArgs e)
    {
        new DeleteDebtorWindow(_debtorRepository, 0, false, _remittanceRepository, selectedDebtor.Id).ShowDialog();
    }

    private void btnEditRemitance_Click(object sender, RoutedEventArgs e)
    {
        new AddRemitance(_remittanceRepository, _operatorUserRepository, true, selectedRemittance.Id, new AddUpdateRemittanceModel
        {
            RemittanceNumber = selectedRemittance.RemittanceNumber,
            InsurancePayment = selectedRemittance.InsurancePayment,
            OperatorUserId = selectedRemittance.OperatorUserId,
            OrganizationPayment = selectedRemittance.OrganizationPayment,
            ProductInsuranceNumber = selectedRemittance.ProductInsuranceNumber,
            ReceviedCommission = selectedRemittance.ReceviedCommission,
            SubmitDate = selectedRemittance.SubmitDate,
            TransforPayment = selectedRemittance.InsurancePayment,
            UserCut = selectedRemittance.UserCut,
            TaxPayment = selectedRemittance.TaxPayment
        }).ShowDialog();
    }

    private void txtSearchRemitanceById_TextChanged(object sender, TextChangedEventArgs e)
    {
        //if (string.IsNullOrEmpty(txtSearchDebtorsByName.Text))
        //{
        //    dgReport.ItemsSource = remittanceList;
        //    return;
        //}
        //todo
    }

}
