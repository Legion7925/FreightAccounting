using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Remittances;
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
    private readonly IOperatorUserRepository operatorUserRepository;
    private readonly IRemittanceRepository remittanceRepository;
    private bool _isEdit;
    private readonly int _remittanceId;
    public static IEnumerable<OperatorUser> userList = new List<OperatorUser>();

    public AddRemitance(IRemittanceRepository remittanceRepository, IOperatorUserRepository operatorUserRepository,
        bool isEdit, int? remitanceId, AddUpdateRemittanceModel? addUpdateRemittanceModel)
    {
        InitializeComponent();
        this.remittanceRepository = remittanceRepository;
        this.operatorUserRepository = operatorUserRepository;
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
                await remittanceRepository.UpdateRemittance(_remittanceId, new AddUpdateRemittanceModel
                {
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt32(txtTranforPayment.Text),
                    OrganizationPayment = Convert.ToInt32(doubleTranforPayment * .12),
                    InsurancePayment = Convert.ToInt32(doubleTranforPayment * .05),
                    TaxPayment = Convert.ToInt32(doubleTranforPayment * .01),
                    SubmitDate = DateTime.Now,
                    OperatorUserId = ((KeyValuePair<int, string>)cbSubmitUser.SelectedItem).Key,
                    UserCut = 0,
                    ReceviedCommission = Convert.ToInt32(txtReceviedCommission.Text),

                });
            }
            else
            {
                await remittanceRepository.AddRemittance(new AddUpdateRemittanceModel
                {
                    RemittanceNumber = txtNumberRemmitance.Text,
                    TransforPayment = Convert.ToInt32(txtTranforPayment.Text),
                    OrganizationPayment = Convert.ToInt32(doubleTranforPayment * .12),
                    InsurancePayment = Convert.ToInt32(doubleTranforPayment * .05),
                    TaxPayment = Convert.ToInt32(doubleTranforPayment * .01),
                    SubmitDate = DateTime.Now,
                    OperatorUserId = 0,
                    UserCut = 0,
                    ReceviedCommission = Convert.ToInt32(txtReceviedCommission.Text),
                });
            }
        }
        catch (AppException ne)
        {

        }
        catch (Exception)
        {

        }
    }

    private async Task GetUserList()
    {
        try
        {
            var userDictionary = new Dictionary<int, string>();
            userList = await operatorUserRepository.GetOperatorUsers();
            if (userList.Any())
            {
                foreach (var item in userList)
                {
                    userDictionary.Add(item.Id, item.Name + " " + item.Family);
                }
            }
            cbSubmitUser.ItemsSource = userDictionary;
        }
        catch (AppException ne) { }
        catch (Exception) { }
    }

    private void btnAddUser_Click(object sender, RoutedEventArgs e)
    {
        new AddUser(operatorUserRepository).ShowDialog();
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
}
