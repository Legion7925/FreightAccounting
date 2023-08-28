using ControlPlateText;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
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
                    //show snackbar
                    //todo update table 
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
                    //todo show snackbar
                    //todo update table 
                }
                Close();
            }
            catch (AppException ax)
            {
                //todo show snackbar
            }
            catch (Exception ex)
            {
                //todo show snackbar
                //todo log error
            }
        }

        private bool ValidateInputs()
        {
            if (!ControlPlate.ControlPleat(txtPlate.PlateText))
            {
                MessageBox.Show("لطفا شماره پلاک را به درستی وارد کنید");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDriverFirstName.Text) || string.IsNullOrWhiteSpace(txtDestination.Text))
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
}
