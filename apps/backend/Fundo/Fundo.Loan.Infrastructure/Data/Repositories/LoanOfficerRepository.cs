using Fundo.Loan.Application.Abstractions;
using Fundo.Loan.Domain.Entities;
using Fundo.Loan.Infrastructure.Data.Common;

namespace Fundo.Loan.Infrastructure.Data.Repositories;

public class LoanOfficerRepository(ApplicationDbContext context) : Repository<LoanOfficer>(context), ILoanOfficerRepository
{
}
