using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Expenses;
using FreightAccounting.WPF.Helper;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for AddExpenseWindow.xaml
/// </summary>
public partial class AddExpenseWindow : Window
{
    private readonly IExpensesRepository expensesRepository;
    private readonly bool isEdit;
    private readonly int expenseId;

    public AddExpenseWindow(IExpensesRepository expensesRepository , bool isEdit  , int? expenseId , AddUpdateExpenseModel? expenseModel)
    {
        InitializeComponent();
        this.expensesRepository = expensesRepository;
        this.isEdit = isEdit;
        if (isEdit)
        {
            this.expenseId = expenseId!.Value;
            txtExpensesAmount.Text = expenseModel!.ExpensesAmount.ToString();
            dpExpense.DisplayDate = new Mohsen.PersianDate(expenseModel.SubmitDate);
        }
    }

    private async void btnSubmitExpense_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var valid = ValidateInputs();

            btnSubmitExpense.IsEnabled = false;

            if (valid is not true)
            {
                btnSubmitExpense.IsEnabled = true;
                return;
            }

            if (isEdit)
            {
                await expensesRepository.UpdateExpense(expenseId, new AddUpdateExpenseModel
                {
                    ExpensesAmount = Convert.ToInt64(txtExpensesAmount.Text.Replace(",", "")),
                    SubmitDate = dpExpense.SelectedDate.ToDateTime(),
                });
                NotificationEventsManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            }
            else
            {
                await expensesRepository.AddExpense(new AddUpdateExpenseModel
                {
                    ExpensesAmount = Convert.ToInt64(txtExpensesAmount.Text.Replace(",", "")),
                    SubmitDate = dpExpense.SelectedDate.ToDateTime(),
                });
                NotificationEventsManager.OnShowMessage("مورد جدید با موفقیت اضافه شد!", MessageTypeEnum.Success);
            }
            CartableEventsManager.OnUpdateExpensesDatagrid();
            Close();

        }
        catch (AppException ne)
        {
            NotificationEventsManager.OnShowMessage(ne.Message, MessageTypeEnum.Warning);
            btnSubmitExpense.IsEnabled = true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در ثبت اطلاعات خطایی رخ داده است", MessageTypeEnum.Error);
            btnSubmitExpense.IsEnabled = true;
        }
    }

    private bool ValidateInputs()
    {
        var isNumeric = int.TryParse(txtExpensesAmount.Text.Replace(",", ""), out _);
        if (!isNumeric)
        {
            MessageBox.Show("لطفا مقدار مخارج را به درستی وارد کنید");
            return false;
        }
        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void txtExpensesAmount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        // Remove existing separators (commas) from the user's input
        string userInput = txtExpensesAmount.Text.Replace(",", "");

        // Convert the user's input to a numeric value
        if (int.TryParse(userInput, out int amount))
        {
            // Apply the separator to the numeric value
            string formattedAmount = amount.ToString("N0");

            // Update the TextBox with the formatted text
            if (txtExpensesAmount.Text != formattedAmount)
            {
                txtExpensesAmount.Text = formattedAmount;

                // Set the caret position at the end of the TextBox
                txtExpensesAmount.CaretIndex = txtExpensesAmount.Text.Length;
            }
        }
    }
}
