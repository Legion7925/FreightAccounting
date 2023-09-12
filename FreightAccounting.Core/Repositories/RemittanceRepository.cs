using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Debtors;
using FreightAccounting.Core.Model.Remittances;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core.Repositories;


public class RemittanceRepository : IRemittanceRepository
{
    private readonly FreightAccountingContext _context;

    public RemittanceRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    /// <summary>
    /// تعداد کل گزارشات برای صفحه بندی
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="operatorUserId"></param>
    /// <returns></returns>
    public int GetRemittanceReportCount(DateTime startDate, DateTime endDate, int? operatorUserId)
    {
        var remittanceList = _context.Remittances
           .AsNoTracking()
           .Include(x => x.OperatorUser)
           .Where(r => r.SubmitDate.Date >= startDate.Date
           && r.SubmitDate.Date <= endDate.Date);

        //اگر آیدی کاربر نال نباشه بر اساس اون کاربر گزارش میاد بیرون
        if (operatorUserId is not null)
        {
            remittanceList = remittanceList.Where(r => r.OperatorUserId == operatorUserId);
        }

        return remittanceList.Count();
    }

    /// <summary>
    /// گزارش بین دو تاریخ
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public RemittanceReportModel GetRemittancesBetweenDates(RemittanceQueryParameter queryParameters)
    {
        var remittanceList = _context.Remittances
            .AsNoTracking()
            .Include(x => x.OperatorUser)
            .Where(r => r.SubmitDate.Date >= queryParameters.StartDate.Date
            && r.SubmitDate.Date <= queryParameters.EndDate.Date).Select(r => new RemittanceEntityReportModel
            {
                RemittanceNumber = r.RemittanceNumber,
                Id = r.Id,
                ReceviedCommission = r.ReceviedCommission,
                InsurancePayment = r.InsurancePayment,
                NetProfit = r.NetProfit,
                OperatorUserId = r.OperatorUserId,
                SubmittedUsername = r.OperatorUser!.Name + " " + r.OperatorUser.Family,
                OrganizationPayment = r.OrganizationPayment,
                SubmitDate = r.SubmitDate,
                TaxPayment = r.TaxPayment,
                TransforPayment = r.TransforPayment,
                ProductInsurancePayment = r.ProductInsurancePayment,
                UserCut = r.UserCut,
                IsUserCutEnteredByHand = r.IsUserCutEnteredByHand,
            });

        var remittanceReportModel = new RemittanceReportModel();

        //اگر آیدی کاربر نال نباشه بر اساس اون کاربر گزارش میاد بیرون
        if (queryParameters.OperatorUserId is not null)
        {
            remittanceList = remittanceList.Where(r => r.OperatorUserId == queryParameters.OperatorUserId);
        }

        //اینجا پر میکنیم اینارو چون که وقتی از خط پایین رد بشه صفحه بندی میشه مقادیر درست در نمیاد دیگه
        remittanceReportModel.SumIncome = remittanceList.Select(r => r.ReceviedCommission).Sum();
        remittanceReportModel.SumNetProfit = remittanceList.Select(r => r.NetProfit).Sum();
        remittanceReportModel.SumUserCut = remittanceList.Select(r => r.UserCut).Sum();
        remittanceReportModel.SumInsurancePayment = remittanceList.Select(r => r.InsurancePayment).Sum();
        remittanceReportModel.SumTaxPayment = remittanceList.Select(r => r.TaxPayment).Sum();
        remittanceReportModel.SumProductInsurance = remittanceList.Select(r => r.ProductInsurancePayment).Sum();
        remittanceReportModel.SumOrganizationPayment = remittanceList.Select(r => r.OrganizationPayment).Sum();


        remittanceList = remittanceList
            .OrderByDescending(i => i.SubmitDate)
            .Skip((queryParameters.Page - 1) * queryParameters.Size)
            .Take(queryParameters.Size);


        remittanceReportModel.Remittances = remittanceList.ToList();

        for (int i = 0; i < remittanceReportModel.Remittances.Count; i++)
        {
            remittanceReportModel.Remittances[i].RowNumber = i + 1 + ((queryParameters.Page - 1) * queryParameters.Size);
        }

        return remittanceReportModel;

    }

    /// <summary>
    /// یک رکورد بر اساس شماره بارنامه
    /// </summary>
    /// <returns></returns>
    public IEnumerable<RemittanceEntityReportModel> GetRemittanceByRettmianceNumber(string remittanceNumber)
    {
        var remittance = _context.Remittances.Where(r => r.RemittanceNumber.Contains(remittanceNumber)).Select(r=> new RemittanceEntityReportModel
        {
            RemittanceNumber = r.RemittanceNumber,
            Id = r.Id,
            ReceviedCommission = r.ReceviedCommission,
            InsurancePayment = r.InsurancePayment,
            NetProfit = r.NetProfit,
            OperatorUserId = r.OperatorUserId,
            SubmittedUsername = r.OperatorUser!.Name + " " + r.OperatorUser.Family,
            OrganizationPayment = r.OrganizationPayment,
            SubmitDate = r.SubmitDate,
            TaxPayment = r.TaxPayment,
            TransforPayment = r.TransforPayment,
            ProductInsurancePayment = r.ProductInsurancePayment,
            UserCut = r.UserCut,
            IsUserCutEnteredByHand = r.IsUserCutEnteredByHand,
        }).ToList();

        if (remittance is null)
        {
            throw new AppException("حواله با شماره بارنامه وارد شده یافت نشد");
        }

        for (int i = 0; i < remittance.Count; i++)
        {
            remittance[i].RowNumber = i + 1;
        }

        return remittance;
    }

    public async Task AddRemittance(AddUpdateRemittanceModel remittanceModel)
    {
        //محاسبه جمع مالیات ها که بعدا از کمیسیون کسر خواهد شد
        var dailyIncome = CalculateTaxes(remittanceModel);
        //سود خالص
        var netProfit = remittanceModel.ReceviedCommission - dailyIncome;
        var remittance = new Remittance
        {
            InsurancePayment = remittanceModel.InsurancePayment,
            RemittanceNumber = remittanceModel.RemittanceNumber,
            OperatorUserId = remittanceModel.OperatorUserId,
            //محاسبه سود خالص با کم کردن درصد ها از میزان کمیسیون
            NetProfit = netProfit,
            UserCut = remittanceModel.UserCut,
            ReceviedCommission = remittanceModel.ReceviedCommission,
            OrganizationPayment = remittanceModel.OrganizationPayment,
            TaxPayment = remittanceModel.TaxPayment,
            TransforPayment = remittanceModel.TransforPayment,
            SubmitDate = remittanceModel.SubmitDate,
            ProductInsurancePayment = remittanceModel.ProductInsurancePayment,
            IsUserCutEnteredByHand = remittanceModel.IsUserCutEnteredByHand
        };

        await _context.Remittances.AddAsync(remittance);

        await UpdateExpenseIncome(remittanceModel.SubmitDate, netProfit);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateRemittance(int remittanceId, AddUpdateRemittanceModel remittanceModel)
    {
        var remittance = await GetRemittanceById(remittanceId);

        var dailyIncome = CalculateTaxes(remittanceModel);

        var NewNetProfit = remittanceModel.ReceviedCommission - dailyIncome;

        //تفاوت سود خالص قبلی با جدید آیا سود خالص اضافه شده یا کم شده
        var netProfitDifference = NewNetProfit - remittance.NetProfit;

        if(remittance.NetProfit > NewNetProfit)
        {
            await UpdateExpenseIncome(remittanceModel.SubmitDate, netProfitDifference);
        }
        else
        {
            await UpdateExpenseIncome(remittanceModel.SubmitDate, netProfitDifference);
        }

        remittance.InsurancePayment = remittanceModel.InsurancePayment;
        remittance.RemittanceNumber = remittanceModel.RemittanceNumber;
        remittance.OperatorUserId = remittanceModel.OperatorUserId;
        //محاسبه سود خالص با کم کردن درصد ها از میزان کمیسیون
        remittance.NetProfit = NewNetProfit;
        remittance.UserCut = remittanceModel.UserCut;
        remittance.ReceviedCommission = remittanceModel.ReceviedCommission;
        remittance.OrganizationPayment = remittanceModel.OrganizationPayment;
        remittance.TaxPayment = remittanceModel.TaxPayment;
        remittance.TransforPayment = remittanceModel.TransforPayment;
        remittance.SubmitDate = remittanceModel.SubmitDate;
        remittance.ProductInsurancePayment = remittanceModel.ProductInsurancePayment;
        remittance.IsUserCutEnteredByHand = remittanceModel.IsUserCutEnteredByHand;


        await _context.SaveChangesAsync();
    }

    public async Task DeleteRemittance(int remittanceId)
    {
        var remittance = await GetRemittanceById(remittanceId);

        await UpdateExpenseIncome(remittance.SubmitDate, -remittance.NetProfit);

        _context.Remittances.Remove(remittance);
        await _context.SaveChangesAsync();
    }

    private async Task<Remittance> GetRemittanceById(int remittanceId)
    {
        var remittance = await _context.Remittances.FirstOrDefaultAsync(i => i.Id == remittanceId);
        if (remittance == null)
        {
            throw new AppException("حواله مورد نظر یافت نشد");
        }
        return remittance;
    }

    private async Task UpdateExpenseIncome(DateTime submitDate , long netProfit)
    {
        //اگر تو تاریخی که داره حواله ثبت میشه مخارج هم ثبت شده باشه میریم در آمد محاسبه شده
        //اون روز را آپدیت میکنیم چون یه سود خالص جدید اضافه شده به تاریخ روز
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.SubmitDate == submitDate);
        if (expense is not null)
        {
            expense.Income = expense.Income + netProfit;
        }
    }

    /// <summary>
    /// جمع مبالغ بیمه ، مالیات ، دارایی و غیره 
    /// </summary>
    /// <param name="remittanceModel"></param>
    /// <returns></returns>
    private long CalculateTaxes(AddUpdateRemittanceModel remittanceModel)
    {
        var sum = remittanceModel.InsurancePayment +
            remittanceModel.ProductInsurancePayment +
            remittanceModel.TaxPayment + remittanceModel.OrganizationPayment + remittanceModel.UserCut;
        return sum;
    }
}
