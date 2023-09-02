using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.WPF.Helper;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for SubmitDebtWindow.xaml
/// </summary>
public partial class SubmitDebtWindow : Window
{
    private readonly IDebtorRepository debtorRepository;
    private readonly int debtorId;

    public SubmitDebtWindow(IDebtorRepository debtorRepository , int debtorId)
    {
        InitializeComponent();
        this.debtorRepository = debtorRepository;
        this.debtorId = debtorId;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        dpSubmit.DisplayDate = Mohsen.PersianDate.Today;
    }

    private async void btnSubmitPayment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var valid = ValidateInputs();

            btnSubmitPayment.IsEnabled = false;

            if (valid is not true)
            {
                btnSubmitPayment.IsEnabled = true;
                return;
            }

            await debtorRepository.SubmitPayment(debtorId , dpSubmit.SelectedDate.ToDateTime());
            NotificationEventsManager.OnShowMessage("پرداخت بدهی با موفقیت انجام شد", MessageTypeEnum.Success);
            CartableEventsManager.OnUpdateDebtorDatagrid();
            this.Close();
        }
        catch (AppException ax)
        {
            NotificationEventsManager.OnShowMessage(ax.Message, MessageTypeEnum.Warning);
            btnSubmitPayment.IsEnabled = true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در عملیات ثبت خطایی رخ داده", MessageTypeEnum.Error);
            btnSubmitPayment.IsEnabled = true;
        }
    }

    private bool ValidateInputs()
    {
        //if(!DateTime.TryParse(dpSubmit.DisplayDate.ToString() , out _))
        //{
        //    NotificationEventsManager.OnShowMessage("لطفا تاریخ را با فرمت درست وارد کنید", MessageTypeEnum.Warning);
        //    return false;
        //}
        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
