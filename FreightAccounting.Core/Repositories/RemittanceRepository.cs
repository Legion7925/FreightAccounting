﻿using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.Core.Model;
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
    /// گزارش بین دو تاریخ
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public async Task<GetRemittanceModel> GetRemittancesBetweenDates(RemittanceQueryParameter queryParameters)
    {
        var remittanceList = _context.Remittances
            .AsNoTracking()
            .Include(x => x.OperatorUser)
            .Where(r => r.SubmitDate >= queryParameters.StartDate
            && r.SubmitDate <= queryParameters.EndDate).Select(r => new Remittance
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
                UserCut = r.UserCut
            });

        //اگر آیدی کاربر نال نباشه بر اساس اون کاربر گزارش میاد بیرون
        if (queryParameters.OperatorUserId is not null)
        {
            remittanceList = remittanceList.Where(r => r.OperatorUserId == queryParameters.OperatorUserId);
        }

        remittanceList = remittanceList
            .Skip((queryParameters.Page - 1) * queryParameters.Size)
            .Take(queryParameters.Size);

        return new GetRemittanceModel
        {
            Remittances = await remittanceList.ToListAsync()
        };
    }

    /// <summary>
    /// یک رکورد بر اساس شماره بارنامه
    /// </summary>
    /// <returns></returns>
    public async Task<Remittance> GetRemittanceByRettmianceNumber(string remittanceNumber)
    {
        var remittance = await _context.Remittances.Where(r => r.RemittanceNumber.Contains(remittanceNumber)).FirstOrDefaultAsync();
        if (remittance is null)
        {
            throw new AppException("حواله با شماره بارنامه وارد شده یافت نشد");
        }
        return remittance;
    }

    public async Task AddRemittance(AddUpdateRemittanceModel remittanceModel)
    {
        //محاسبه جمع مالیات ها که بعدا از کمیسیون کسر خواهد شد
        var dailyIncome = CalculateTaxes(remittanceModel);
        var remittance = new Remittance
        {
            InsurancePayment = remittanceModel.InsurancePayment,
            RemittanceNumber = remittanceModel.RemittanceNumber,
            OperatorUserId = remittanceModel.OperatorUserId,
            //محاسبه سود خالص با کم کردن درصد ها از میزان کمیسیون
            NetProfit = remittanceModel.ReceviedCommission - dailyIncome,
            UserCut = remittanceModel.UserCut,
            ReceviedCommission = remittanceModel.ReceviedCommission,
            OrganizationPayment = remittanceModel.OrganizationPayment,
            TaxPayment = remittanceModel.TaxPayment,
            TransforPayment = remittanceModel.TransforPayment,
            SubmitDate = remittanceModel.SubmitDate
        };

        await _context.Remittances.AddAsync(remittance);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRemittance(int remittanceId, AddUpdateRemittanceModel remittanceModel)
    {
        var remittance = await GetRemittanceById(remittanceId);
        var dailyIncome = CalculateTaxes(remittanceModel);

        remittance.InsurancePayment = remittanceModel.InsurancePayment;
        remittance.RemittanceNumber = remittanceModel.RemittanceNumber;
        remittance.OperatorUserId = remittanceModel.OperatorUserId;
        //محاسبه سود خالص با کم کردن درصد ها از میزان کمیسیون
        remittance.NetProfit = remittanceModel.ReceviedCommission - dailyIncome;
        remittance.UserCut = remittanceModel.UserCut;
        remittance.ReceviedCommission = remittanceModel.ReceviedCommission;
        remittance.OrganizationPayment = remittanceModel.OrganizationPayment;
        remittance.TaxPayment = remittanceModel.TaxPayment;
        remittance.TransforPayment = remittanceModel.TransforPayment;
        remittance.SubmitDate = remittanceModel.SubmitDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRemittance(int remittanceId)
    {
        var remittance = await GetRemittanceById(remittanceId);

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

    /// <summary>
    /// جمع مبالغ بیمه ، مالیات ، دارایی و غیره 
    /// </summary>
    /// <param name="remittanceModel"></param>
    /// <returns></returns>
    private int CalculateTaxes(AddUpdateRemittanceModel remittanceModel)
    {
        return remittanceModel.InsurancePayment + remittanceModel.TaxPayment + remittanceModel.OrganizationPayment + remittanceModel.UserCut;
    }
}
