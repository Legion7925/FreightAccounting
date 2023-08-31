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
using PersianDate.Standard;
using Stimulsoft.Report;
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
    private RemittanceEntityReportModel selectedRemittance = new RemittanceEntityReportModel() { RemittanceNumber = string.Empty };
    private ExpenseEntityReportModel selectedExpense = new ExpenseEntityReportModel();
    //private int _selectedId;

    public MainWindow(IDebtorRepository debtorRepository,
        IRemittanceRepository remittanceRepository,
        IOperatorUserRepository operatorUserRepository,
        IExpensesRepository expensesRepository)
    {
        InitializeComponent();
        _debtorRepository = debtorRepository;
        _remittanceRepository = remittanceRepository;
        _operatorUserRepository = operatorUserRepository;
        _expensesRepository = expensesRepository;

        NotificationEventsManager.showMessage += ShowSnackbarMessage;
        CartableEventsManager.updateDebtorDatagrid += FillDebtorDatagrid;
        CartableEventsManager.updateRemittanceDatagrid += FillRemitanceDatagrid;
        CartableEventsManager.updateExpensesDatagrid += btnReportExpenses_Click!;
        CartableEventsManager.updateRemittanceDatagrid += btnReportRemitance_Click!;

        dpExpensesReportStart.SelectedDate = Mohsen.PersianDate.Today.AddDays(-3);
        dpExpensesReportEnd.SelectedDate = Mohsen.PersianDate.Today;
        dpRemittanceStart.SelectedDate = Mohsen.PersianDate.Today.AddDays(-3);
        dpRemittanceEnd.SelectedDate = Mohsen.PersianDate.Today;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        FillPaginationComboboxes();
        FillDebtorDatagrid(null, null);
        btnReportExpenses_Click(null!, null!);
        btnReportRemitance_Click(null!, null!);
        FillOperatorUsersCombobox();
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
    private void FillDebtorDatagrid(object? sender, EventArgs? e)
    {
        try
        {
            debtorsList = _debtorRepository.GetDebtors(new QueryParameters());
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
                FillDebtorDatagrid(null, null);
                break;
            case 1:
                dgDebtorsReport.ItemsSource = debtorsList = debtorsList.Where(d => d.Paid == true).ToList();
                break;
            case 2:
                dgDebtorsReport.ItemsSource = debtorsList = debtorsList.Where(d => d.Paid == false).ToList();
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
        new DeleteDebtorWindow(_debtorRepository, selectedDebtor.Id).ShowDialog();
    }

    private void btnSubmitPayment_Click(object sender, RoutedEventArgs e)
    {
        new SubmitDebtWindow(_debtorRepository, selectedDebtor.Id).ShowDialog();
    }

    private void btnPrintDebtorsReport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!debtorsList.Any())
            {
                ShowSnackbarMessage("داده ای برای نمایش پرینت در این تاریخ موجود نیست", MessageTypeEnum.Information);
                btnPrintDebtorsReport.IsEnabled = true;
                return;
            }

            var stiReport = new StiReport();
            StiOptions.Dictionary.BusinessObjects.MaxLevel = 1;
            stiReport.Load(@"Report\DebtorsReport.mrt");
            stiReport.RegData("لیست بدهکاران", debtorsList);
            stiReport.Show();
            btnPrintDebtorsReport.IsEnabled = true;

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


    #endregion Debtors

    #region Expenses

    private int _expensesPageSize = 5;

    private int _expensesPageIndex = 1;

    private double _expensesTotalCount = 0;

    private double _expensesTotalpage = 0;

    private void btnReportExpenses_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            expensesReportModel.Expenses = new List<ExpenseEntityReportModel>();
            dgExpenses.ItemsSource = null;

            _expensesTotalCount = _expensesRepository
                .GetExpenseReportCount(dpExpensesReportStart.SelectedDate.ToDateTime(), dpExpensesReportEnd.SelectedDate.ToDateTime());

            if (_expensesTotalCount is 0)
            {
                ShowSnackbarMessage("داده ای برای نمایش یافت نشد", MessageTypeEnum.Information);
                lblTotalExpenses.Text = "-";
                lblTotalIncomeWithExpenses.Text = "-";
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

    private void GetPaginatedExpenseReport()
    {
        try
        {
            expensesReportModel = _expensesRepository.GetExpensesReport(
            new ExpensesQueryParameters
            {
                StartDate = dpExpensesReportStart.SelectedDate.ToDateTime(),
                EndDate = dpExpensesReportEnd.SelectedDate.ToDateTime(),
                Page = _expensesPageIndex,
                Size = _expensesPageSize
            });

            dgExpenses.ItemsSource = expensesReportModel.Expenses;
            lblTotalExpenses.Text = expensesReportModel.TotalExpensesAmount.ToString();
            lblTotalIncomeWithExpenses.Text = expensesReportModel.TotalIncome.ToString();
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

    private void FillExpensesDatagrid(object? sender, EventArgs? e)
    {
        _expensesPageSize = Convert.ToInt32(cmbExpensePaginationSize.SelectedValue);
        _expensesTotalpage = Math.Ceiling(_expensesTotalCount / _expensesPageSize);
        if (_expensesTotalpage == 0)
            _expensesTotalpage = 1;

        GetPaginatedExpenseReport();

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
        lblExpensePageNumberInfo.Text = $"{((_expensesPageIndex - 1) * _expensesPageSize) + 1} - {((_expensesPageIndex - 1) * _expensesPageSize) + expensesReportModel.Expenses.Count()}";
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

    private void cmbExpensePaginationSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!expensesReportModel.Expenses.Any())
            return;
        _expensesPageIndex = 1;
        FillExpensesDatagrid(null, null);
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
        FillExpensesDatagrid(null, null);
    }

    private void btnExpenseLastPage_Click(object sender, RoutedEventArgs e)
    {
        _expensesPageIndex = Convert.ToInt32(_expensesTotalpage);
        FillExpensesDatagrid(null, null);
    }

    private void btnPrintExpenseReport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnPrintExpenseReport.IsEnabled = false;
            var result = _expensesRepository.GetExpensesReport(new ExpensesQueryParameters
            {
                Page = 1,
                Size = int.MaxValue,
                StartDate = dpExpensesReportStart.SelectedDate.ToDateTime(),
                EndDate = dpExpensesReportEnd.SelectedDate.ToDateTime()
            });


            if (result.Expenses.Any() is not true)
            {
                ShowSnackbarMessage("داده ای برای نمایش پرینت در این تاریخ موجود نیست", MessageTypeEnum.Information);
                btnPrintExpenseReport.IsEnabled = true;
                return;
            }

            var stiReport = new StiReport();
            StiOptions.Dictionary.BusinessObjects.MaxLevel = 1;
            stiReport.Load(@"Report\ExpensesReport.mrt");
            stiReport.RegData("لیست مخارج", result.Expenses);
            stiReport.RegData("مجموع مخارج", result.TotalExpensesAmount);
            stiReport.RegData("مجموع درآمد خالص", result.TotalIncome);
            stiReport.RegData("تاریخ", new
            {
                تاریخ_شروع = dpExpensesReportStart.SelectedDate.ToDateTime().ToFa(),
                تاریخ_پایان = dpExpensesReportEnd.SelectedDate.ToDateTime().ToFa(),
            });

            stiReport.Show();
            btnPrintExpenseReport.IsEnabled = true;

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


    #endregion Expenses

    #region Remittance

    private int _remitancePageSize = 5;

    private int _remitancePageIndex = 1;

    private double _remitanceTotalCount = 0;

    private double _remitanceTotalpage = 0;

    private void btnReportRemitance_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            int? operatorUserId = ((KeyValuePair<int, string>?)cbUserFilter.SelectedItem)?.Key;

            _remitanceTotalCount = _remittanceRepository.GetRemittanceReportCount(
                dpRemittanceStart.SelectedDate.ToDateTime(),
                dpRemittanceEnd.SelectedDate.ToDateTime()
                , operatorUserId);


            if (_remitanceTotalCount == 0)
            {
                ShowSnackbarMessage("داده ای برای نمایش یافت نشد", MessageTypeEnum.Information);
                return;
            };
            FillRemitanceDatagrid(null, null);
            gridRemitancePagination.IsEnabled = true;
        }
        catch (AppException ax)
        {
            ShowSnackbarMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage("در واکشی اطلاعات خطایی رخ داده است", MessageTypeEnum.Error);
            Logger.LogException(ex);
        }
    }

    private void FillRemitanceDatagrid(object? sender, EventArgs? e)
    {
        _remitancePageSize = Convert.ToInt32(cmbRemitancePaginationSize.SelectedValue);
        _remitanceTotalpage = Math.Ceiling(_remitanceTotalCount / _remitancePageSize);
        if (_remitanceTotalpage == 0)
            _remitanceTotalpage = 1;

        GetPaginatedRemitanceReport();

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
        if (_remitancePageIndex == _remitanceTotalpage)
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
        lblRemitancePageNumberInfo.Text = $"{((_remitancePageIndex - 1) * _remitancePageSize) + 1} - {((_remitancePageIndex - 1) * _remitancePageSize) + remittanceReportModel.Remittances.Count()}";
        dgReport.ItemsSource = remittanceReportModel.Remittances;
    }

    private void GetPaginatedRemitanceReport()
    {
        try
        {
            int? operatorUserId = ((KeyValuePair<int, string>?)cbUserFilter.SelectedItem)?.Key;
            remittanceReportModel = _remittanceRepository.GetRemittancesBetweenDates(
            new RemittanceQueryParameter
            {
                StartDate = dpRemittanceStart.SelectedDate.ToDateTime(),
                EndDate = dpRemittanceEnd.SelectedDate.ToDateTime(),
                OperatorUserId = operatorUserId
            });

            lblTotalIncomeBasedOnComission.Text = remittanceReportModel.SumIncome.ToString();
            lblTotalInsurancePayment.Text = remittanceReportModel.SumInsurancePayment.ToString();
            lblTotalTaxPayment.Text = remittanceReportModel.SumTaxPayment.ToString();
            lblTotalNetPorfit.Text = remittanceReportModel.SumNetProfit.ToString();
            lblTotalUserCut.Text = remittanceReportModel.SumUserCut.ToString();
            //todo اگر نیاز است که جمه پورسانت کاربران نمایش داده شود اینجا باید استک پنل آن ویزیبل شود
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
            selectedRemittance = dgReport.SelectedItem as RemittanceEntityReportModel ?? new RemittanceEntityReportModel() { RemittanceNumber = string.Empty };
    }

    private void btnDeleteRemitance_Click(object sender, RoutedEventArgs e)
    {
        new DeleteRemitanceWindow(_remittanceRepository, selectedRemittance.Id).ShowDialog();
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
            TransforPayment = selectedRemittance.TransforPayment,
            UserCut = selectedRemittance.UserCut,
            TaxPayment = selectedRemittance.TaxPayment
        }).ShowDialog();
    }

    private void FillOperatorUsersCombobox()
    {
        try
        {
            var userDictionary = new Dictionary<int, string>();
            _userList = _operatorUserRepository.GetOperatorUsers();
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


    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
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


    private void FillPaginationComboboxes()
    {
        Dictionary<int, string> paginationSizeValuePairs = new Dictionary<int, string>
            {
                { 0, "10" },
                { 1, "20" },
                { 2, "30" }
            };
        cmbExpensePaginationSize.ItemsSource = paginationSizeValuePairs;
        cmbRemitancePaginationSize.ItemsSource = paginationSizeValuePairs;
    }

    private void btnPrintRemitanceReport_Click(object sender, RoutedEventArgs e)
    {
        var report = _remittanceRepository.GetRemittancesBetweenDates(new RemittanceQueryParameter
        {
            StartDate = dpRemittanceStart.SelectedDate.ToDateTime(),
            EndDate = dpRemittanceEnd.SelectedDate.ToDateTime(),
            OperatorUserId = null,
            Page = 1,
            Size = int.MaxValue
        });
        if (!report.Remittances.Any())
        {
            ShowSnackbarMessage("داده ای برای نمایش پرینت در این تاریخ موجود نیست", MessageTypeEnum.Information);
            btnPrintRemitanceReport.IsEnabled = true;
            return;
        }

        var stiReport = new StiReport();
        StiOptions.Dictionary.BusinessObjects.MaxLevel = 1;
        stiReport.Load(@"Report\RemittanceReport.mrt");
        stiReport.RegData("لیست حواله ها", report.Remittances);
        stiReport.RegData("تاریخ گزارش", new
        {
            تاریخ_شروع = dpExpensesReportStart.SelectedDate.ToDateTime().ToFa(),
            تاریخ_پایان = dpExpensesReportEnd.SelectedDate.ToDateTime().ToFa(),
        });
        stiReport.RegData("مجموع پرداختی و درآمد ها", new
        {
            report.SumIncome,
            report.SumNetProfit,
            report.SumInsurancePayment,
            report.SumTaxPayment,
            report.SumUserCut
        });
        stiReport.Show();
        btnPrintDebtorsReport.IsEnabled = true;
    }

    private void btnSearchNumberRemitance_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtSearchRemitanceById.Text))
        {
            ShowSnackbarMessage("شماره حواله را ابتدا وارد کنید", MessageTypeEnum.Warning);
            return;
        }
        var s = remittanceReportModel.Remittances.Where(b => b.RemittanceNumber.Contains(txtSearchRemitanceById.Text)).ToList();
        if (s.Count is 0)
        {
            dgReport.ItemsSource = null;
            ShowSnackbarMessage("برای این شماره ،حواله ای ثبت نشده", MessageTypeEnum.Information);
        }
        dgReport.ItemsSource = s;
    }

    private void btnReportRemoveFilter_Click(object sender, RoutedEventArgs e)
    {
        txtSearchRemitanceById.Text = string.Empty;
        cbUserFilter.Text = string.Empty;
        btnReportRemitance_Click(null!, null!);
        dgReport.ItemsSource = remittanceReportModel.Remittances;
    }
    #endregion Remittance
}
