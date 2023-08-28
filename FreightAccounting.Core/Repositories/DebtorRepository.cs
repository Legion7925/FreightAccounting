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
            PlateNumber = debtorModel.PlateNumber,
            Destination = debtorModel.Destination,
            DriverLastName = debtorModel.DriverLastName,
            DriverFirstName = debtorModel.DriverFirstName,
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
        debtor.DriverLastName = debtorModel.DriverLastName;
        debtor.DriverFirstName = debtorModel.DriverFirstName;
        debtor.DebtAmount = debtorModel.DebtAmount;
        debtor.PhoneNumber = debtorModel.PhoneNumber;
        debtor.PlateNumber = debtorModel.PlateNumber;

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
