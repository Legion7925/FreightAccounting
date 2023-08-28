using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
using FreightAccounting.WPF.Helper;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
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
    private readonly IDebtorRepository debtorRepository;
    private IEnumerable<DebtorReportModel> debtorsList = new List<DebtorReportModel>();
    private DebtorReportModel selectedDebtor = new DebtorReportModel();

    public MainWindow(IDebtorRepository debtorRepository)
    {
        InitializeComponent();
        this.debtorRepository = debtorRepository;

        NotificationManager.showMessage += ShowSnackbarMessage;
        CartableEvents.updateDebtorDatagrid += FillDebtorDatagrid;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        FillDebtorDatagrid(null, null);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {

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

    }

    private void dgReport_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {

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
        var addDebtorWindow = new AddDebtorWindow(debtorRepository, false, null, null);
        addDebtorWindow.ShowDialog();
    }

    /// <summary>
    /// پر کردن دیتاگرید بدهکاران
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void FillDebtorDatagrid(object? sender , EventArgs? e)
    {
        try
        {
            debtorsList = await debtorRepository.GetDebtors(new QueryParameters());
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
        dgDebtorsReport.ItemsSource = debtorsList.Where(d=> (d.DriverFirstName + d.DriverLastName).Contains(txtSearchDebtorsByName.Text)).ToList();
    }
    /// <summary>
    /// باز کردن پنجره ویرایش بدهکار
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnEditDebtor_Click(object sender, RoutedEventArgs e)
    {
        new AddDebtorWindow(debtorRepository, true, selectedDebtor.Id, new AddUpdateDebtorModel
        {
            DebtAmount = selectedDebtor.DebtAmount,
            Destination = selectedDebtor.Destination,
            DriverFirstName = selectedDebtor.DriverFirstName,
            DriverLastName = selectedDebtor.DriverLastName,
            PlateNumber= selectedDebtor.PlateNumber,
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
        new DeleteDebtorWindow(debtorRepository, selectedDebtor.Id).ShowDialog();
    }
    #endregion Debtors

}
