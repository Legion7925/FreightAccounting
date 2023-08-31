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
    private int _userCut;

    public AddRemitance(IRemittanceRepository remittanceRepository, IOperatorUserRepository operatorUserRepository,
        bool isEdit, int? remitanceId, AddUpdateRemittanceModel? addUpdateRemittanceModel)
    {
        InitializeComponent();
        _remittanceRepository = remittanceRepository;
        _operatorUserRepository = operatorUserRepository;
        _isEdit = isEdit;

        if (_isEdit)
        {
            _remittanceId = remitanceId!.Value;
            txtNumberRemmitance.Text = addUpdateRemittanceModel!.RemittanceNumber;
            txtTranforPayment.Text = addUpdateRemittanceModel.TransforPayment.ToString();
            txtOrganizationPayment.Text = addUpdateRemittanceModel.OrganizationPayment.ToString();
            txtInsurancePayment.Text = addUpdateRemittanceModel.InsurancePayment.ToString();
            txtTaxPayment.Text = addUpdateRemittanceModel.TaxPayment.ToString();
            dpDate.SelectedDate = new Mohsen.PersianDate(addUpdateRemittanceModel.SubmitDate);
            cbUserCut.SelectedIndex = addUpdateRemittanceModel.UserCut;
            txtUserCut.Text = addUpdateRemittanceModel.UserCut.ToString();
            txtProductInsurance.Text = addUpdateRemittanceModel.ProductInsuranceNumber?.ToString() ?? string.Empty; //todo
            txtReceviedCommission.Text = addUpdateRemittanceModel.ReceviedCommission.ToString();
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        GetUserList();
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

            var doubleTranforPayment = double.Parse(txtTranforPayment.Text);
            if (_isEdit)
            {
                await _remittanceRepository.UpdateRemittance(_remittanceId, new AddUpdateRemittanceModel
                {
                    ProductInsuranceNumber = Convert.ToInt32(txtProductInsurance.Text),
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt32(txtTranforPayment.Text),
                    OrganizationPayment = Convert.ToInt32(doubleTranforPayment * .12),
                    InsurancePayment = Convert.ToInt32(doubleTranforPayment * .05),
                    TaxPayment = Convert.ToInt32(doubleTranforPayment * .01),
                    SubmitDate = dpDate.SelectedDate.ToDateTime(),
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = _userCut,
                    ReceviedCommission = Convert.ToInt32(txtReceviedCommission.Text)
                });
                NotificationEventsManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            }
            else
            {
                await _remittanceRepository.AddRemittance(new AddUpdateRemittanceModel
                {
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt32(txtTranforPayment.Text),
                    OrganizationPayment = Convert.ToInt32(doubleTranforPayment * .12),
                    InsurancePayment = Convert.ToInt32(doubleTranforPayment * .05),
                    TaxPayment = Convert.ToInt32(doubleTranforPayment * .01),
                    SubmitDate = dpDate.SelectedDate.ToDateTime(),
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = _userCut,
                    ProductInsuranceNumber = Convert.ToInt32(txtProductInsurance.Text),
                    ReceviedCommission = Convert.ToInt32(txtReceviedCommission.Text),
                });
                NotificationEventsManager.OnShowMessage("حواله جدید با موفقیت اضافه شد!", MessageTypeEnum.Success);
            }
            CartableEventsManager.OnUpdateRemittanceDatagrid();
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

    private void GetUserList()
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
        //if(selectedUserOperatorId is 0)
        //{
        //    MessageBox.Show("لطفا کاربر ثبت کننده را انتخاب کنید");
        //    return false;
        //}

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

        return true;
    }

    private void cbUserCut_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtTranforPayment.Text))
        {
            //MessageBox.Show("مقدار کرایه نمیتواند خالی باشد");
            return;
        }
        else
        {
            var doubleTranforPayment = double.Parse(txtTranforPayment.Text);
            switch (cbUserCut.SelectedIndex)
            {
                case 0:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .005);
                    txtUserCut.Text = _userCut.ToString();
                    break;
                case 1:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .03);
                    txtUserCut.Text = _userCut.ToString();
                    break;
                case 2:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .05);
                    txtUserCut.Text = _userCut.ToString();
                    break;
            }
        }
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

        if (string.IsNullOrWhiteSpace(txtTranforPayment.Text))
            return;
        var doubleTranforPayment = int.Parse(txtTranforPayment.Text);
        var organizationPayment = AppSession.AppSettings.OrganizationPercentage * doubleTranforPayment / 100;
        txtOrganizationPayment.Text = organizationPayment.ToString();

        var taxPayment = AppSession.AppSettings.TaxPercentage * doubleTranforPayment / 100;
        txtTaxPayment.Text = taxPayment.ToString();

        var insurePayment = AppSession.AppSettings.InsurancePercentage * doubleTranforPayment / 100;
        txtInsurancePayment.Text = insurePayment.ToString();

        if (string.IsNullOrWhiteSpace(txtProductInsurance.Text))
            return;
        var productInsurance = int.Parse(txtProductInsurance.Text);
        var totalPayment = organizationPayment + taxPayment + insurePayment + productInsurance + _userCut;

        if (string.IsNullOrWhiteSpace(txtReceviedCommission.Text))
            return;
        var receveComision = int.Parse(txtReceviedCommission.Text);
        txtNetProfit.Text = (receveComision - totalPayment).ToString();
    }
}
