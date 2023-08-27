using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
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
    /// لیست همه ی بدهکاران
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Debtor>> GetDebtors(QueryParameters queryParameters)
    {
        return await _context.Debtors.AsNoTracking()
            .Skip((queryParameters.Page - 1) * queryParameters.Size)
            .Take(queryParameters.Size)
            .ToArrayAsync();
    }

    public async Task AddDebtor(AddUpdateDebtorModel debtorModel)
    {
        await _context.Debtors.AddAsync(new Debtor
        {
            Destination = debtorModel.Destination,
            DriverFamilyName = debtorModel.DriverFamilyName,
            DriverName = debtorModel.DriverName,
            DebtAmount = debtorModel.DebtAmount,
            PhoneNumber = debtorModel.PhoneNumber,
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// پرداخت انجام شد
    /// </summary>
    /// <param name="debtorId"></param>
    /// <returns></returns>
    public async Task SubmitPayment(int debtorId)
    {
        var debtor = await GetDebtorById(debtorId);

        debtor.PaymentDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateDebtor(int debtorId, AddUpdateDebtorModel debtorModel)
    {
        var debtor = await GetDebtorById(debtorId);

        debtor.Destination = debtorModel.Destination;
        debtor.DriverFamilyName = debtorModel.DriverFamilyName;
        debtor.DriverName = debtorModel.DriverName;
        debtor.DebtAmount = debtorModel.DebtAmount;
        debtor.PhoneNumber = debtorModel.PhoneNumber;

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
