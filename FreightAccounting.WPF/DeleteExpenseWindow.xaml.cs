using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.WPF.Helper;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for DeleteExpenseWindow.xaml
/// </summary>
public partial class DeleteExpenseWindow : Window
{
    private readonly IExpensesRepository expensesRepository;
    private readonly int expenseId;

    public DeleteExpenseWindow(IExpensesRepository expensesRepository , int expenseId)
    {
        InitializeComponent();
        this.expensesRepository = expensesRepository;
        this.expenseId = expenseId;
    }

    private async void btnSubmitDelete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await expensesRepository.DeleteExpense(expenseId);
            NotificationEventsManager.OnShowMessage("حذف مخارج با موفقیت انجام شد!", MessageTypeEnum.Success);
            CartableEventsManager.OnUpdateExpensesDatagrid();
            Close();
        }
        catch (AppException ax)
        {
            NotificationEventsManager.OnShowMessage(ax.Message, MessageTypeEnum.Warning);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            NotificationEventsManager.OnShowMessage("در عملیات حذف خطایی رخ داده", MessageTypeEnum.Error);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
