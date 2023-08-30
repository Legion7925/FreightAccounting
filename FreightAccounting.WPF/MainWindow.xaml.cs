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
using System.Threading.Tasks;
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
    private static IEnumerable<OperatorUser> _userList = new List<OperatorUser>();
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
        CartableEventsManager.updateExpensesDatagrid += btnReportExpenses_Click!;
        _expensesRepository = expensesRepository;

        dpExpensesReportStart.SelectedDate = Mohsen.PersianDate.Today.AddDays(-2);
        dpExpensesReportEnd.SelectedDate = Mohsen.PersianDate.Today;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        FillDebtorDatagrid(null, null);
        btnReportExpenses_Click(null!, null!);
        FillPaginationComboboxes();
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

    private int _expensesPageSize = 5;

    private int _expensesPageIndex = 1;

    private double _expensesTotalCount = 0;

    private double _expensesTotalpage = 0;

    private async Task GetPaginatedExpenseReport()
    {
        try
        {
            expensesReportModel = await _expensesRepository.GetExpensesReport(
            new ExpensesQueryParameters
            {
                StartDate = dpExpensesReportStart.SelectedDate.ToDateTime(),
                EndDate = dpExpensesReportEnd.SelectedDate.ToDateTime(),
                Page = _expensesPageIndex,
                Size = _expensesPageSize
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

    private async void FillExpensesDatagrid(object? sender, EventArgs? e)
    {
        _expensesPageSize = Convert.ToInt32(cmbExpensePaginationSize.SelectedValue);
        _expensesTotalpage = Math.Ceiling(_expensesTotalCount / _expensesPageSize);
        if (_expensesTotalpage == 0)
            _expensesTotalpage = 1;

        await GetPaginatedExpenseReport();

        if (_expensesTotalCount == 0)
        {
            btnExpenseLastPage.IsEnabled = false;
            btnExpenseNextPage.IsEnabled = false;
            btnExpensePreviousPage.IsEnabled = false;
            btnExpenseFirstPage.IsEnabled = false;
        }
        else
        {
            btnExpenseLastPage.IsEnabled = true;
            btnExpenseNextPage.IsEnabled = true;
            btnExpensePreviousPage.IsEnabled = true;
            btnExpenseFirstPage.IsEnabled = true;
        }
        if (_expensesPageIndex == 1)
        {
            btnExpensePreviousPage.IsEnabled = false;
            btnExpenseFirstPage.IsEnabled = false;
        }
        if (_expensesPageIndex == _expensesTotalpage)
        {
            btnExpenseLastPage.IsEnabled = false;
            btnExpenseNextPage.IsEnabled = false;
        }
        else
        {
            btnExpenseLastPage.IsEnabled = true;
            btnExpenseNextPage.IsEnabled = true;
        }
        lblExpenseTotalCount.Text = _expensesTotalCount.ToString();
        lblExpensePageNumberInfo.Text = $"{((_expensesPageIndex - 1) * _expensesPageSize)} - {((_expensesPageIndex - 1) * _expensesPageSize) + expensesReportModel.Expenses.Count()}";
        dgExpenses.ItemsSource = expensesReportModel.Expenses;
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

    private async void btnReportExpenses_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            expensesReportModel.Expenses = new List<ExpenseEntityReportModel>();
            dgExpenses.ItemsSource = null;

            _expensesTotalCount = await _expensesRepository
                .GetExpenseReportCount(dpExpensesReportStart.SelectedDate.ToDateTime(), dpExpensesReportEnd.SelectedDate.ToDateTime());

            if (_expensesTotalCount is 0)
            {
                ShowSnackbarMessage("داده ای برای نمایش یافت نشد", MessageTypeEnum.Information);
                return;
            }

            FillExpensesDatagrid(null, null);

            gridExpensePagination.IsEnabled = true;

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

    private void cmbExpensePaginationSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!expensesReportModel.Expenses.Any())
            return;
        _expensesPageIndex = 1;
        FillExpensesDatagrid(null,null);
    }

    private void btnExpenseFirstPage_Click(object sender, RoutedEventArgs e)
    {
        _expensesPageIndex = 1;
        FillExpensesDatagrid(null, null);
    }

    private void btnExpensePreviousPage_Click(object sender, RoutedEventArgs e)
    {
        _expensesPageIndex--;
        FillExpensesDatagrid(null, null);
    }

    private void btnExpenseNextPage_Click(object sender, RoutedEventArgs e)
    {
        _expensesPageIndex++;
        FillExpensesDatagrid(null , null);
    }

    private void btnExpenseLastPage_Click(object sender, RoutedEventArgs e)
    {
        _expensesPageIndex = Convert.ToInt32(_expensesTotalpage);
        FillExpensesDatagrid(null, null);
    }



    #endregion Expenses

    #region Remittance

    private int _remitancePageSize = 5;

    private int _remitancePageIndex = 1;

    private double _remitanceTotalCount = 0;

    private double _remitanceTotalpage = 0;
    private async void FillRemitanceDatagrid(object? sender, EventArgs? e)
    {
        _remitancePageSize = Convert.ToInt32(cmbRemitancePaginationSize.SelectedValue);
        _expensesTotalpage = Math.Ceiling(_remitanceTotalCount / _remitancePageSize);
        if (_remitanceTotalpage == 0)
            _remitanceTotalpage = 1;

        await GetPaginatedRemitanceReport();

        if (_remitanceTotalCount == 0)
        {
            btnRemitanceLastPage.IsEnabled = false;
            btnRemitanceNextPage.IsEnabled = false;
            btnRemitancePreviousPage.IsEnabled = false;
            btnRemitanceFirstPage.IsEnabled = false;
        }
        else
        {
            btnRemitanceLastPage.IsEnabled = true;
            btnRemitanceNextPage.IsEnabled = true;
            btnRemitancePreviousPage.IsEnabled = true;
            btnRemitanceFirstPage.IsEnabled = true;
        }
        if (_remitancePageIndex == 1)
        {
            btnRemitancePreviousPage.IsEnabled = false;
            btnRemitanceFirstPage.IsEnabled = false;
        }
        if (_remitancePageIndex == _expensesTotalpage)
        {
            btnRemitanceLastPage.IsEnabled = false;
            btnRemitanceNextPage.IsEnabled = false;
        }
        else
        {
            btnRemitanceLastPage.IsEnabled = true;
            btnRemitanceNextPage.IsEnabled = true;
        }
        lblRemitanceTotalCount.Text = _remitanceTotalCount.ToString();
        lblRemitancePageNumberInfo.Text = $"{((_remitancePageIndex - 1) * _remitancePageSize)} - {((_remitancePageIndex - 1) * _remitancePageSize) + remittanceReportModel.Remittances.Count()}";
        dgReport.ItemsSource = remittanceReportModel.Remittances;
    }

    private async Task GetPaginatedRemitanceReport()
    {
        try
        {
            remittanceReportModel = await _remittanceRepository.GetRemittancesBetweenDates(
            new RemittanceQueryParameter
            {
                StartDate = dpRemittanceStart.SelectedDate.ToDateTime().AddDays(-3),
                EndDate = dpRemittanceEnd.SelectedDate.ToDateTime()
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

    private async Task GetUserList()
    {
        try
        {
            var userDictionary = new Dictionary<int, string>();
            _userList = await _operatorUserRepository.GetOperatorUsers();
            if (_userList.Any())
            {
                foreach (var item in _userList)
                {
                    userDictionary.Add(item.Id, item.Name + " " + item.Family);
                }
            }
            cbUserFilter.ItemsSource = userDictionary;
        }
        catch (AppException ne)
        {
            NotificationEventsManager.OnShowMessage(ne.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در واکشی اطلاعات خطایی رخ داده است", MessageTypeEnum.Error);
        }
    }
    #endregion Remittance

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
    }

    

    private void FillPaginationComboboxes()
    {
        Dictionary<int, string> paginationSizeValuePairs = new Dictionary<int, string>
            {
                { 0, "10" },
                { 1, "20" },
                { 2, "30" }
            };
        cmbExpensePaginationSize.ItemsSource = paginationSizeValuePairs;
    }

    

    private async void btnReportRemitance_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //remittanceReportModel.Remittances = new List<Remittance>();
            //dgReport.ItemsSource = null;

            //_remitanceTotalCount = await _remittanceRepository.GetRemittancesBetweenDates(new RemittanceQueryParameter
            //{
            //    StartDate = dpRemittanceStart.SelectedDate.ToDateTime(),
            //    EndDate = dpRemittanceEnd.SelectedDate.ToDateTime(),
            //    OperatorUserId = ((KeyValuePair<int, string>)cbUserFilter.SelectedItem).Key

            //});

            //_remitanceTotalCount = await _remittanceRepository.

            //if(_remitanceTotalCount = 0)
            //{
            //    ShowSnackbarMessage("داده ای برای نمایش یافت نشد", MessageTypeEnum.Information);
            //    return;
            //}
            //FillRemitanceDatagrid(null, null);
            //gridRemitancePagination.IsEnabled = true;

            //expensesReportModel.Expenses = new List<ExpenseEntityReportModel>();
            //dgExpenses.ItemsSource = null;

            //_expensesTotalCount = await _expensesRepository
            //    .GetExpenseReportCount(dpExpensesReportStart.SelectedDate.ToDateTime(), dpExpensesReportEnd.SelectedDate.ToDateTime());


        }
        catch(AppException ax)
        {
            ShowSnackbarMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception)
        {
            ShowSnackbarMessage("در واکشی اطلاعات خطایی رخ داده است", MessageTypeEnum.Error);
        }
    }

    private void btnRemitanceLastPage_Click(object sender, RoutedEventArgs e)
    {
        _remitancePageIndex = Convert.ToInt32(_remitanceTotalpage);
        FillRemitanceDatagrid(null, null);
    }

    private void btnRemitanceNextPage_Click(object sender, RoutedEventArgs e)
    {
        _remitancePageIndex++;
        FillRemitanceDatagrid(null, null);
    }

    private void btnRemitancePreviousPage_Click(object sender, RoutedEventArgs e)
    {
        _remitancePageIndex--;
        FillRemitanceDatagrid(null, null);
    }

    private void btnRemitanceFirstPage_Click(object sender, RoutedEventArgs e)
    {
        _remitancePageIndex = 1;
        FillRemitanceDatagrid(null, null);
    }

    private void cmbRemitancePaginationSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!remittanceReportModel.Remittances.Any())
            return;
        _remitancePageIndex = 1;
        FillRemitanceDatagrid(null, null);
    }
}
