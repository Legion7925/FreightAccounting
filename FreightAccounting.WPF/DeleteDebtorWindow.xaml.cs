using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.WPF.Helper;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for DeleteDebtorWindow.xaml
/// </summary>
public partial class DeleteDebtorWindow : Window
{
    private readonly IDebtorRepository debtorRepository;
    private readonly int debtorId;


    public DeleteDebtorWindow(IDebtorRepository debtorRepository, int debtorId)
    {
        InitializeComponent();
        this.debtorRepository = debtorRepository;
        this.debtorId = debtorId;
    }

    private async void btnSubmitDelete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await debtorRepository.DeleteDebtor(debtorId);
            NotificationManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            CartableEvents.OnUpdateDebtorDatagrid();
            Close();
        }
        catch (AppException ax)
        {
            NotificationManager.OnShowMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationManager.OnShowMessage("در عملیات حذف خطایی رخ داده", MessageTypeEnum.Error);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
