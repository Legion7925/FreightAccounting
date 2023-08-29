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
            txtInsurancePercentage.Text = AppSession.AppSettings.InsurancePercentage.ToString();
            txtTaxPercentage.Text = AppSession.AppSettings.TaxPercentage.ToString();
            txtOrganizationPercentage.Text = AppSession.AppSettings.OrganizationPercentage.ToString();
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
                AppSession.AppSettings.InsurancePercentage = Convert.ToInt32(txtInsurancePercentage.Text);
                AppSession.AppSettings.TaxPercentage = Convert.ToInt32(txtTaxPercentage.Text);
                AppSession.AppSettings.OrganizationPercentage = Convert.ToInt32(txtOrganizationPercentage.Text);

                JsonSerializerSettings serilizerSetting = new JsonSerializerSettings();
                serilizerSetting.NullValueHandling = NullValueHandling.Ignore;
                var setting = JsonConvert.SerializeObject(AppSession.AppSettings, serilizerSetting);

                File.WriteAllText($@"{Environment.CurrentDirectory}\Setting.json", setting);

                NotificationEventsManager.OnShowMessage("تنظیمات با موفقیت ذخیره شد", MessageTypeEnum.Success);
                Close();

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                NotificationEventsManager.OnShowMessage("در عملیات ذخیره تنظیمات خطایی رخ داده", MessageTypeEnum.Error);
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

            if(!int.TryParse(txtInsurancePercentage.Text, out _) ||
               !int.TryParse(txtTaxPercentage.Text, out _) ||
               !int.TryParse(txtOrganizationPercentage.Text, out _))
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
