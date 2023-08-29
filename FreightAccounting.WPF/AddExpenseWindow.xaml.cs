using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Expenses;
using FreightAccounting.WPF.Helper;
using System;
using System.Windows;

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
                    ExpensesAmount = Convert.ToInt32(txtExpensesAmount.Text),
                    SubmitDate = dpExpense.SelectedDate.ToDateTime(),
                });
                NotificationEventsManager.OnShowMessage("عملیات ویرایش با موفقیت انجام شد!", MessageTypeEnum.Success);
            }
            else
            {
                await expensesRepository.AddExpense(new AddUpdateExpenseModel
                {
                    ExpensesAmount = Convert.ToInt32(txtExpensesAmount.Text),
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
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در ثبت اطلاعات خطایی رخ داده است", MessageTypeEnum.Error);
        }
    }

    private bool ValidateInputs()
    {
        var isNumeric = int.TryParse(txtExpensesAmount.Text, out _);
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
}
