using FreightAccounting.Core.Exception;
using FreightAccounting.WPF.Helper;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FreightAccounting.WPF
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtInsurancePercentage.Text = (AppSession.AppSettings.InsurancePercentage * 10).ToString();
            txtTaxPercentage.Text = (AppSession.AppSettings.TaxPercentage * 10).ToString();
            txtOrganizationPercentage.Text = (AppSession.AppSettings.OrganizationPercentage * 10).ToString();
            txtUserCutPercentage.Text = (AppSession.AppSettings.UserCutPercentage * 10).ToString();
        }

        private void btnSubmitSettings_Click(object sender, RoutedEventArgs e)
        {
            var valid = ValidateInputs();

            btnSubmitSettings.IsEnabled = false;

            if (valid is not true)
            {
                btnSubmitSettings.IsEnabled = true;
                return;
            }

            try
            {
                AppSession.AppSettings.InsurancePercentage = Convert.ToDouble(txtInsurancePercentage.Text) / 10;
                AppSession.AppSettings.TaxPercentage = Convert.ToDouble(txtTaxPercentage.Text) / 10;
                AppSession.AppSettings.OrganizationPercentage = Convert.ToDouble(txtOrganizationPercentage.Text) / 10;
                AppSession.AppSettings.UserCutPercentage = Convert.ToDouble(txtUserCutPercentage.Text) / 10;

                JsonSerializerSettings serilizerSetting = new JsonSerializerSettings();
                serilizerSetting.NullValueHandling = NullValueHandling.Ignore;
                var setting = JsonConvert.SerializeObject(AppSession.AppSettings, serilizerSetting);

                File.WriteAllText($@"{Environment.CurrentDirectory}\Setting.json", setting);

                NotificationEventsManager.OnShowMessage("تنظیمات با موفقیت ذخیره شد", MessageTypeEnum.Success);
                Close();

            }
            catch (AppException ne)
            {
                NotificationEventsManager.OnShowMessage(ne.Message, MessageTypeEnum.Warning);
                btnSubmitSettings.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                NotificationEventsManager.OnShowMessage("در عملیات ذخیره تنظیمات خطایی رخ داده", MessageTypeEnum.Error);
                btnSubmitSettings.IsEnabled = true;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtInsurancePercentage.Text) ||
                string.IsNullOrEmpty(txtTaxPercentage.Text) ||
                string.IsNullOrEmpty(txtOrganizationPercentage.Text))
            {
                NotificationEventsManager.OnShowMessage("لطفا همه ی فیلد ها را پر کنید!", MessageTypeEnum.Warning);
                return false;
            }

            if (!double.TryParse(txtInsurancePercentage.Text, out _) ||
               !double.TryParse(txtTaxPercentage.Text, out _) ||
               !double.TryParse(txtOrganizationPercentage.Text, out _) ||
               !double.TryParse(txtUserCutPercentage.Text, out _))
            {
                NotificationEventsManager.OnShowMessage("لطفا همه ی فیلد ها را با فرمت عددی وارد کنید!", MessageTypeEnum.Warning);
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
