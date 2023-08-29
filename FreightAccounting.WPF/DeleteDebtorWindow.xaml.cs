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
    private readonly IRemittanceRepository remitanceRepository;
    private readonly int debtorId;
    private readonly int remitanceId;
    private bool _isDebtors;


    public DeleteDebtorWindow(IDebtorRepository debtorRepository, int debtorId, bool isDebtors, IRemittanceRepository remittanceRepository, int remitanceId)
    {
        InitializeComponent();
        this.debtorRepository = debtorRepository;
        this.debtorId = debtorId;
        this.remitanceRepository = remittanceRepository;
        _isDebtors = isDebtors;
        this.remitanceId = remitanceId;
    }

    private async void btnSubmitDelete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_isDebtors)
            {
                await debtorRepository.DeleteDebtor(debtorId);
                NotificationEventsManager.OnShowMessage("حذف نسیه با موفقیت انجام شد!", MessageTypeEnum.Success);
                CartableEventsManager.OnUpdateDebtorDatagrid();
            }
            else
            {
                await remitanceRepository.DeleteRemittance(remitanceId);
                NotificationEventsManager.OnShowMessage("حذف حواله با موفقیت انجام شد!", MessageTypeEnum.Success);
                CartableEventsManager.OnUpdateRemittanceDatagrid();
            }

            Close();
        }
        catch (AppException ax)
        {
            NotificationEventsManager.OnShowMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در عملیات حذف خطایی رخ داده", MessageTypeEnum.Error);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
