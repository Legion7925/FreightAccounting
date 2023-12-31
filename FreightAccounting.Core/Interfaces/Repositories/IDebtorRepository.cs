﻿using FreightAccounting.Core.Entities;
using FreightAccounting.Core.Model.Common;
using FreightAccounting.Core.Model.Debtors;

namespace FreightAccounting.Core.Interfaces.Repositories;

public interface IDebtorRepository
{
    Task AddDebtor(AddUpdateDebtorModel debtorModel);
    Task DeleteDebtor(int debtorId);
    DebtorReportModel GetDebtors(DebtorsQueryParameters queryParameters);
    DebtorReportModel GetDebtorsByName(string searchedName);
    Task SubmitPayment(int debtorId, DateTime paymentDate);
    Task UpdateDebtor(int debtorId, AddUpdateDebtorModel debtorModel);
    int GetDebtorsReportCount(bool? paid, DateTime startDate, DateTime endDate);
}
