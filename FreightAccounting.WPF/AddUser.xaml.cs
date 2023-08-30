using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Repositories;
using FreightAccounting.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FreightAccounting.WPF
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        private readonly IOperatorUserRepository operatorUserRepository;
        public event EventHandler<MessageTypeEnum>? ShowMessage;

        public AddUser(IOperatorUserRepository operatorUserRepository)
        {
            InitializeComponent();
            this.operatorUserRepository = operatorUserRepository;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                NotificationEventsManager.OnShowMessage("نام کاربر را وارد کنید", MessageTypeEnum.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtFamily.Text))
            {
                NotificationEventsManager.OnShowMessage("نام خانوادکی کاربر را وارد کنید", MessageTypeEnum.Warning);
                return false;
            }
            return true;
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var valid = ValidateInput();
                if (!valid) return;
                await operatorUserRepository.AddOperatorUser(new OperatorUser
                {
                    Name = txtName.Text,
                    Family = txtFamily.Text
                });
                NotificationEventsManager.OnShowMessage("کاربر با موفقیت ثبت شد", MessageTypeEnum.Success);
                CartableEventsManager.OnUpdateOperatorUserCombobox();
                Close();
            }
            catch (AppException ne)
            {
                NotificationEventsManager.OnShowMessage(ne.Message, MessageTypeEnum.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                NotificationEventsManager.OnShowMessage("در انجام عملیات خطایی رخ داده است", MessageTypeEnum.Error);
            }
        }
    }
}
