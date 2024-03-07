using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FreightAccounting.WPF
{
    /// <summary>
    /// Interaction logic for DeleteRemitanceWindow.xaml
    /// </summary>
    public partial class DeleteRemitanceWindow : Window
    {
        private readonly IRemittanceRepository remitanceRepository;
        private readonly int _remitanceId;
        public DeleteRemitanceWindow(IRemittanceRepository remittanceRepository, int remitanceId)
        {
            InitializeComponent();
            this.remitanceRepository = remittanceRepository;
            this._remitanceId = remitanceId;
        }

        private void btnSubmitDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 remitanceRepository.DeleteRemittance(_remitanceId);
                NotificationEventsManager.OnShowMessage("حذف حواله با موفقیت انجام شد!", MessageTypeEnum.Success);
                CartableEventsManager.OnUpdateRemittanceDatagrid();
                CartableEventsManager.OnUpdateExpensesDatagrid();

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
}
