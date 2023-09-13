using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Debtors;
using Microsoft.EntityFrameworkCore;

namespace FreightAccounting.Core.Repositories;

public class DebtorRepository : IDebtorRepository
{
    private readonly FreightAccountingContext _context;

    public DebtorRepository(FreightAccountingContext context)
    {
        _context = context;
    }

    /// <summary>
    /// تعداد کل گزارش برای صفحه بندی
    /// </summary>
    /// <returns></returns>
    public int GetDebtorsReportCount(bool? paid, DateTime startDate, DateTime endDate)
    {
        var debtors = _context.Debtors.AsNoTracking().Where(r => r.SubmitDate.Date >= startDate.Date
           && r.SubmitDate.Date <= endDate.Date);

        if (paid is not null)
        {
            if (paid is true)
            {
                debtors = debtors.Where(d => d.PaymentDate != null);
            }
            else
            {
                debtors = debtors.Where(d => d.PaymentDate == null);
            }
        }

        return debtors.Count();
    }

    /// <summary>
    /// لیست همه ی بدهکاران
    /// </summary>
    /// <returns></returns>
    public DebtorReportModel GetDebtors(DebtorsQueryParameters queryParameters)
    {
        var debtorsList = _context.Debtors.AsNoTracking().Where(r => r.SubmitDate.Date >= queryParameters.StartDate.Date
            && r.SubmitDate.Date <= queryParameters.EndDate.Date).Select(d => new DebtorEntityReportModel
            {
                Destination = d.Destination,
                DriverFirstName = d.DriverFirstName,
                DriverLastName = d.DriverLastName,
                PlateNumber = d.PlateNumber,
                PaymentDate = d.PaymentDate,
                DebtAmount = d.DebtAmount,
                Id = d.Id,
                PhoneNumber = d.PhoneNumber,
                Description = d.Description,
                SubmitDate = d.SubmitDate,
            });

        if (!string.IsNullOrEmpty(queryParameters.SearchedName))
        {
            var searchedQuery = queryParameters.SearchedName.ToLower().Replace(" ", "");
            debtorsList = debtorsList.Where(d => (d.DriverFirstName.ToLower() + d.DriverLastName.ToLower()).Replace(" " , "")
                .Contains(searchedQuery));
        }

        if (queryParameters.Paid is not null)
        {
            if (queryParameters.Paid.Value is true)
            {
                debtorsList = debtorsList.Where(d => d.PaymentDate != null);
            }
            else
            {
                debtorsList = debtorsList.Where(d => d.PaymentDate == null);
            }
        }

        var debtorsReportModel = new DebtorReportModel();
        debtorsReportModel.TotalDebt = debtorsList.Sum(d => d.DebtAmount);

        debtorsList = debtorsList
           .OrderByDescending(i => i.SubmitDate)
            .Skip((queryParameters.Page - 1) * queryParameters.Size)
            .Take(queryParameters.Size);

        debtorsReportModel.DebtorsList = debtorsList.ToList();

        for (int i = 0; i < debtorsReportModel.DebtorsList.Count; i++)
        {
            debtorsReportModel.DebtorsList[i].RowNumber = i + 1 + ((queryParameters.Page - 1) * queryParameters.Size);
        }

        return debtorsReportModel;
    }

    public DebtorReportModel GetDebtorsByName(string searchedName)
    {
        var debtorsList = _context.Debtors.AsNoTracking()
            .Where(d => (d.DriverFirstName + d.DriverLastName).Contains(searchedName))
            .Select(d => new DebtorEntityReportModel
            {
                Destination = d.Destination,
                DriverFirstName = d.DriverFirstName,
                DriverLastName = d.DriverLastName,
                PlateNumber = d.PlateNumber,
                PaymentDate = d.PaymentDate,
                DebtAmount = d.DebtAmount,
                Id = d.Id,
                PhoneNumber = d.PhoneNumber,
                Description = d.Description,
                SubmitDate = d.SubmitDate,
            });

        var debtorReportModel = new DebtorReportModel();
        debtorReportModel.DebtorsList = debtorsList.OrderByDescending(d => d.SubmitDate).ToList();
        debtorReportModel.TotalDebt = debtorsList.Sum(d => d.DebtAmount);

        for (int i = 0; i < debtorReportModel.DebtorsList.Count; i++)
        {
            debtorReportModel.DebtorsList[i].RowNumber = i + 1;
        }

        return debtorReportModel;
    }

    public async Task AddDebtor(AddUpdateDebtorModel debtorModel)
    {
        await _context.Debtors.AddAsync(new Debtor
        {
            PlateNumber = debtorModel.PlateNumber,
            Destination = debtorModel.Destination,
            DriverLastName = debtorModel.DriverLastName,
            DriverFirstName = debtorModel.DriverFirstName,
            DebtAmount = debtorModel.DebtAmount,
            PhoneNumber = debtorModel.PhoneNumber,
            Description = debtorModel.Description,
            SubmitDate = debtorModel.SubmitDate,
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// پرداخت انجام شد
    /// </summary>
    /// <param name="debtorId"></param>
    /// <returns></returns>
    public async Task SubmitPayment(int debtorId, DateTime paymentDate)
    {
        var debtor = await GetDebtorById(debtorId);

        debtor.PaymentDate = paymentDate;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateDebtor(int debtorId, AddUpdateDebtorModel debtorModel)
    {
        var debtor = await GetDebtorById(debtorId);

        debtor.Destination = debtorModel.Destination;
        debtor.DriverLastName = debtorModel.DriverLastName;
        debtor.DriverFirstName = debtorModel.DriverFirstName;
        debtor.DebtAmount = debtorModel.DebtAmount;
        debtor.PhoneNumber = debtorModel.PhoneNumber;
        debtor.PlateNumber = debtorModel.PlateNumber;
        debtor.Description = debtorModel.Description;
        debtor.SubmitDate = debtorModel.SubmitDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDebtor(int debtorId)
    {
        var debtor = await GetDebtorById(debtorId);

        _context.Debtors.Remove(debtor);
        await _context.SaveChangesAsync();
    }

    private async Task<Debtor> GetDebtorById(int debtorId)
    {
        var debtor = await _context.Debtors.FirstOrDefaultAsync(d => d.Id == debtorId);
        if (debtor is null)
        {
            throw new AppException("بدهکار مورد نظر یافت نشد");
        }
        return debtor;
    }
}
