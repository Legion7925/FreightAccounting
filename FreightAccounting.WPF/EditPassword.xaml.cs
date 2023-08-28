
using FreightAccounting.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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
    /// Interaction logic for EditPassword.xaml
    /// </summary>
    public partial class EditPassword : Window
    {

        public event EventHandler<MessageTypeEnum>? ShowMessage;
        public EditPassword()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmitChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var valid = ValidateInput();
                if (!valid) return;
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke(ex.Message, MessageTypeEnum.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(pbOldPass.Password))
            {
                ShowMessage?.Invoke("کلمه عبور قدیمی را وارد کنید", MessageTypeEnum.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(PbNewPass.Password))
            {
                ShowMessage?.Invoke("کلمه عبور جدید را وارد کنید", MessageTypeEnum.Warning);
                return false;
            }
            return true;
        }

    }
}
