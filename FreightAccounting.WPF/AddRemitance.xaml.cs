using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Remittances;
using FreightAccounting.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for AddRemitance.xaml
/// </summary>
public partial class AddRemitance : Window
{
    private readonly IOperatorUserRepository _operatorUserRepository;
    private readonly IRemittanceRepository _remittanceRepository;
    private bool _isEdit;
    private readonly int _remittanceId;
    private static IEnumerable<OperatorUser> _userList = new List<OperatorUser>();
    private long _userCut;

    private long _contentUserCut;

    public AddRemitance(IRemittanceRepository remittanceRepository, IOperatorUserRepository operatorUserRepository,
        bool isEdit, int? remitanceId, AddUpdateRemittanceModel? addUpdateRemittanceModel)
    {
        InitializeComponent();
        CartableEventsManager.updateOperatorUserCombobox += GetUserList;
        _remittanceRepository = remittanceRepository;
        _operatorUserRepository = operatorUserRepository;
        _isEdit = isEdit;
        GetUserList(null! , null!);

        if (_isEdit)
        {
            if (addUpdateRemittanceModel!.UserCut == 1)
            {
                _contentUserCut = 1;
            }
            else if(addUpdateRemittanceModel!.UserCut == 0)
            {
                _contentUserCut = 0;
            }
            else
            {
                var o = addUpdateRemittanceModel!.UserCut * 100 / addUpdateRemittanceModel.TransforPayment;
                if(o == 0)
                {
                    _contentUserCut = 1;
                }
                else
                {
                    _contentUserCut = o;
                }
            }

            var selectedUser = _userList.FirstOrDefault(u => u.Id == addUpdateRemittanceModel.OperatorUserId);
            cbSubmitUser.SelectedItem = new KeyValuePair<int, string>(selectedUser!.Id, selectedUser.Name + " " + selectedUser.Family);
            switch (_contentUserCut)
            {
                case 0:
                    txtUserCut.Text = 0.ToString();
                    cbUserCut.SelectedIndex = 0;
                    _userCut = 0;
                    break;
                case 1:
                    cbUserCut.SelectedIndex = 1;
                    txtUserCut.Text = (addUpdateRemittanceModel.TransforPayment / 200).ToString();
                    _userCut = addUpdateRemittanceModel.TransforPayment / 200;
                    break;
                case 3:
                    cbUserCut.SelectedIndex = 2;
                    txtUserCut.Text = (addUpdateRemittanceModel.TransforPayment * 3 / 100).ToString();
                    _userCut = addUpdateRemittanceModel.TransforPayment * 3 / 100;
                    break;
                case 5:
                    cbUserCut.SelectedIndex = 3;
                    txtUserCut.Text = (addUpdateRemittanceModel.TransforPayment * 5 / 100).ToString();
                    _userCut = addUpdateRemittanceModel.TransforPayment * 5 / 100;
                    break;
            }
            _remittanceId = remitanceId!.Value;
            txtNumberRemmitance.Text = addUpdateRemittanceModel!.RemittanceNumber;
            txtTranforPayment.Text = addUpdateRemittanceModel.TransforPayment.ToString();
            txtOrganizationPayment.Text = addUpdateRemittanceModel.OrganizationPayment.ToString();
            txtInsurancePayment.Text = addUpdateRemittanceModel.InsurancePayment.ToString();
            txtTaxPayment.Text = addUpdateRemittanceModel.TaxPayment.ToString();
            dpDate.SelectedDate = new Mohsen.PersianDate(addUpdateRemittanceModel.SubmitDate);
            //cbUserCut.SelectedIndex = addUpdateRemittanceModel.UserCut;
            //txtUserCut.Text = addUpdateRemittanceModel.UserCut.ToString();
            txtProductInsurance.Text = addUpdateRemittanceModel.ProductInsuranceNumber.ToString(); //todo
            txtReceviedCommission.Text = addUpdateRemittanceModel.ReceviedCommission.ToString();
            CalculateNetProfitAndTaxes(null!, null!);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        cbUserCut.Items.Add(AppSession.AppSettings.UserCutPercentage);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void btnSubmit_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var valid = ValidateInputs();
            if (!valid) return;

            btnSubmit.IsEnabled = false;
            var doubleTranforPayment = double.Parse(txtTranforPayment.Text);
            if (_isEdit)
            {
                await _remittanceRepository.UpdateRemittance(_remittanceId, new AddUpdateRemittanceModel
                {
                    ProductInsuranceNumber = Convert.ToInt64(txtProductInsurance.Text.Replace(",", "")),
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt64(txtTranforPayment.Text.Replace(",", "")),
                    OrganizationPayment = Convert.ToInt64(txtOrganizationPayment.Text.Replace(",", "")),
                    InsurancePayment = Convert.ToInt64(txtInsurancePayment.Text.Replace(",", "")),
                    TaxPayment = Convert.ToInt64(txtTaxPayment.Text.Replace(",", "")),
                    SubmitDate = dpDate.SelectedDate.ToDateTime(),
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = _userCut,
                    ReceviedCommission = Convert.ToInt64(txtReceviedCommission.Text.Replace(",", ""))
                });
                NotificationEventsManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            }
            else
            {
                await _remittanceRepository.AddRemittance(new AddUpdateRemittanceModel
                {
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt64(txtTranforPayment.Text.Replace(",", "")),
                    OrganizationPayment = Convert.ToInt64(txtOrganizationPayment.Text.Replace(",", "")),
                    InsurancePayment = Convert.ToInt64(txtInsurancePayment.Text.Replace(",", "")),
                    TaxPayment = Convert.ToInt64(txtTaxPayment.Text.Replace(",", "")),
                    SubmitDate = dpDate.SelectedDate.ToDateTime(),
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = _userCut,
                    ProductInsuranceNumber = Convert.ToInt64(txtProductInsurance.Text.Replace(",", "")),
                    ReceviedCommission = Convert.ToInt64(txtReceviedCommission.Text.Replace(",", "")),
                });
                NotificationEventsManager.OnShowMessage("حواله جدید با موفقیت اضافه شد!", MessageTypeEnum.Success);
            }
            CartableEventsManager.OnUpdateRemittanceDatagrid();
            CartableEventsManager.OnUpdateExpensesDatagrid();
            btnSubmit.IsEnabled = true;
            Close();
        }
        catch (AppException ne)
        {
            NotificationEventsManager.OnShowMessage(ne.Message, MessageTypeEnum.Warning);
            btnSubmit.IsEnabled = true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در انجام عملیات خطایی رخ داده است", MessageTypeEnum.Error);
            btnSubmit.IsEnabled = true;
        }
    }

    private void GetUserList(object? sender , EventArgs e)
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
            cbSubmitUser.ItemsSource = userDictionary;
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

    private void btnAddUser_Click(object sender, RoutedEventArgs e)
    {
        new AddUser(_operatorUserRepository).ShowDialog();
    }

    private bool ValidateInputs()
    {
        if (cbSubmitUser.SelectedItem is null)
        {
            MessageBox.Show("لطفا کاربر ثبت کننده را انتخاب کنید");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtNumberRemmitance.Text))
        {
            MessageBox.Show("شماره حواله نمیتواند خالی باشد");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtTranforPayment.Text))
        {
            MessageBox.Show("مقدار کرایه نمیتواند خالی باشد");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtReceviedCommission.Text))
        {
            MessageBox.Show("مقدار کمیسیون دریافتی نمیتواند خالی باشد");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtProductInsurance.Text))
        {
            MessageBox.Show("مقدار بیمه کالا نمیتواند خالی باشد");
            return false;
        }

        return true;
    }

    private void cbUserCut_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CalculateNetProfitAndTaxes(null!, null!);
    }

    //private void txtTranforPayment_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    //{
    //    var doubleTranforPayment = Convert.ToInt32(txtTranforPayment.Text);
    //    var organizationPayment = AppSession.AppSettings.OrganizationPercentage * doubleTranforPayment / 100;
    //    txtOrganizationPayment.Text = organizationPayment.ToString();
    //    var taxPayment = AppSession.AppSettings.TaxPercentage * doubleTranforPayment / 100;
    //    txtTaxPayment.Text = taxPayment.ToString();
    //    var insurePayment = AppSession.AppSettings.InsurancePercentage * doubleTranforPayment / 100;
    //    txtInsurancePayment.Text = insurePayment.ToString();
    //}

    private void CalculateNetProfitAndTaxes(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        AddCommaSeparators((TextBox?)sender);

        if (string.IsNullOrWhiteSpace(txtTranforPayment.Text))
            return;

        var doubleTranforPayment = double.Parse(txtTranforPayment.Text.Replace(",", ""));
        switch (cbUserCut.SelectedIndex)
        {
            case 0:
                _userCut = 0;
                txtUserCut.Text = _userCut.ToString();
                break;
            case 1:
                _userCut = Convert.ToInt64(doubleTranforPayment * .01);
                _userCut = _userCut / 2;
                txtUserCut.Text = _userCut.ToString();
                break;
            case 2:
                _userCut = Convert.ToInt64(doubleTranforPayment * .03);
                txtUserCut.Text = _userCut.ToString();
                break;
            case 3:
                _userCut = Convert.ToInt64(doubleTranforPayment * .05);
                txtUserCut.Text = _userCut.ToString();
                break;
            case 4:
                _userCut = Convert.ToInt64(doubleTranforPayment * (AppSession.AppSettings.UserCutPercentage / 100));
                txtUserCut.Text = _userCut.ToString();
                break;
        }

        var organizationPayment = AppSession.AppSettings.OrganizationPercentage * doubleTranforPayment / 100;
        txtOrganizationPayment.Text = organizationPayment.ToString();

        var taxPayment = AppSession.AppSettings.TaxPercentage * doubleTranforPayment / 100;
        txtTaxPayment.Text = taxPayment.ToString();

        var insurePayment = AppSession.AppSettings.InsurancePercentage * doubleTranforPayment / 100;
        txtInsurancePayment.Text = insurePayment.ToString();

        if (string.IsNullOrWhiteSpace(txtProductInsurance.Text.Replace(",", "")))
            return;
        var productInsurance = long.Parse(txtProductInsurance.Text.Replace(",", ""));
        var totalPayment = organizationPayment + taxPayment + insurePayment + productInsurance + _userCut;

        if (string.IsNullOrWhiteSpace(txtReceviedCommission.Text.Replace(",", "")))
            return;
        var receveComision = long.Parse(txtReceviedCommission.Text.Replace(",", ""));
        txtNetProfit.Text = (receveComision - totalPayment).ToString();


    }

    private void CheckTextboxForOnlyNumberInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        // Check if the entered text is a number
        if (!int.TryParse(e.Text, out _))
        {
            e.Handled = true; // Discard the input if it is not a number
        }
    }

    private void AddCommaSeparators(TextBox? textBox)
    {
        if (textBox == null) return;
        if (string.IsNullOrWhiteSpace(textBox.Text))
            return;
        // Remove previous comma separators if any
        string text = textBox.Text.Replace(",", "");

        // Format the text with comma separators
        string formattedText = string.Format("{0:N0}", Convert.ToDecimal(text));

        // Update the text box
        textBox.Text = formattedText;
        textBox.CaretIndex = formattedText.Length; // Maintain the caret position
    }

    private void txtOrganizationPayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is null) return;
        AddCommaSeparators((TextBox)sender);
    }

    private void txtInsurancePayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is null) return;
        AddCommaSeparators((TextBox)sender);
    }

    private void txtTaxPayment_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is null) return;
        AddCommaSeparators((TextBox)sender);
    }

    private void txtUserCut_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is null) return;
        AddCommaSeparators((TextBox)sender);
    }

    private void txtNetProfit_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is null) return;
        AddCommaSeparators((TextBox)sender);
    }
}
