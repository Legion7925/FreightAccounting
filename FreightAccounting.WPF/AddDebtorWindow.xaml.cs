using ControlPlateText;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Debtors;
using FreightAccounting.WPF.Helper;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for AddDebtorWindow.xaml
/// </summary>
public partial class AddDebtorWindow : Window
{
    private readonly IDebtorRepository _debtorRepository;
    private readonly bool _isEdit;
    private readonly int _debtorId;

    public AddDebtorWindow(IDebtorRepository debtorRepository, bool isEdit, int? debtorId, AddUpdateDebtorModel? debtorModel)
    {
        InitializeComponent();
        _debtorRepository = debtorRepository;

        _isEdit = isEdit;

        if (isEdit)
        {
            _debtorId = debtorId!.Value;
            txtDebtAmount.Text = debtorModel!.DebtAmount.ToString();
            txtDestination.Text = debtorModel.Destination;
            txtDriverFirstName.Text = debtorModel.DriverFirstName;
            txtDriverLastName.Text = debtorModel.DriverLastName;
            txtPhoneNumber.Text = debtorModel.PhoneNumber;
            txtPlate.PlateText = debtorModel.PlateNumber;
        }
    }

    private async void btnSubmitDebtor_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var valid = ValidateInputs();

            btnSubmitDebtor.IsEnabled = false;

            if (valid is not true)
            {
                btnSubmitDebtor.IsEnabled = true;
                return;
            }

            if (_isEdit)
            {
                await _debtorRepository.UpdateDebtor(_debtorId, new AddUpdateDebtorModel
                {
                    DriverFirstName = txtDriverFirstName.Text,
                    Destination = txtDestination.Text,
                    DriverLastName = txtDriverLastName.Text,
                    DebtAmount = Convert.ToInt32(txtDebtAmount.Text),
                    PhoneNumber = txtPhoneNumber.Text.ToEnglishNumber(),
                    PlateNumber = txtPlate.PlateText
                });
                NotificationEventsManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            }
            else
            {
                await _debtorRepository.AddDebtor(new AddUpdateDebtorModel
                {
                    DriverFirstName = txtDriverFirstName.Text,
                    Destination = txtDestination.Text,
                    DriverLastName = txtDriverLastName.Text,
                    DebtAmount = Convert.ToInt32(txtDebtAmount.Text),
                    PhoneNumber = txtPhoneNumber.Text,
                    PlateNumber = txtPlate.PlateText.ToEnglishNumber()
                });
                NotificationEventsManager.OnShowMessage("بدهکار جدید با موفقیت اضافه شد!", MessageTypeEnum.Success);
            }
            CartableEventsManager.OnUpdateDebtorDatagrid();
            Close();
        }
        catch (AppException ax)
        {
            NotificationEventsManager.OnShowMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در عملیات ثبت خطایی رخ داده", MessageTypeEnum.Error);
        }
    }

    private bool ValidateInputs()
    {
        if (!ControlPlate.ControlPleat(txtPlate.PlateText))
        {
            MessageBox.Show("لطفا شماره پلاک را به درستی وارد کنید");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtDriverFirstName.Text) || string.IsNullOrWhiteSpace(txtDriverLastName.Text))
        {
            MessageBox.Show("نام و نام خانوادگی نمیتواند خالی باشد");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtDestination.Text))
        {
            MessageBox.Show("لطفا مقصد را وارد کنید");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPlate.PlateText))
        {
            MessageBox.Show("لطفا شماره پلاک را وارد کنید");
            return false;
        }


        var isNumeric = int.TryParse(txtDebtAmount.Text, out _);
        if (!isNumeric)
        {
            MessageBox.Show("لطفا مقدار بدهی را به درستی وارد کنید");
            return false;
        }

        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
