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
            tpDate.Text = addUpdateRemittanceModel.SubmitDate.ToString();
            cbUserCut.SelectedIndex = addUpdateRemittanceModel.UserCut;
            txtUserCut.Text = addUpdateRemittanceModel.UserCut.ToString();
            txtInsurance.Text = addUpdateRemittanceModel.InsurancePayment.ToString(); //todo
            txtReceviedCommission.Text = addUpdateRemittanceModel.ReceviedCommission.ToString();
        }
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await GetUserList();
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
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt32(txtTranforPayment.Text),
                    OrganizationPayment = Convert.ToInt32(doubleTranforPayment * .12),
                    InsurancePayment = Convert.ToInt32(doubleTranforPayment * .05),
                    TaxPayment = Convert.ToInt32(doubleTranforPayment * .01),
                    SubmitDate = DateTime.Now,
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
                    SubmitDate = DateTime.Now,
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = _userCut,
                    ReceviedCommission = Convert.ToInt32(txtReceviedCommission.Text),
                });
                NotificationEventsManager.OnShowMessage("حواله جدید با موفقیت اضافه شد!", MessageTypeEnum.Success);
            }
            CartableEventsManager.OnUpdateDebtorDatagrid();
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

    private async Task GetUserList()
    {
        try
        {
            var userDictionary = new Dictionary<int, string>();
            _userList = await _operatorUserRepository.GetOperatorUsers();
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
            MessageBox.Show("مقدار کرایه نمیتواند خالی باشد");
            return;
        }
        else
        {
            var doubleTranforPayment = double.Parse(txtTranforPayment.Text);
            switch (cbUserCut.SelectedIndex)
            {
                case 0:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .005);
                    break;
                case 1:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .03);
                    break;
                case 2:
                    _userCut = Convert.ToInt32(doubleTranforPayment * .05);
                    break;
            }
        }

    }
}
