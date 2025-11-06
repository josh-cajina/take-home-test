using Fundo.Loan.Application.DTOs;
using Fundo.Loan.Domain.Entities;

namespace Fundo.Loan.Application.Abstractions;
public interface ILoanRequestRepository : IRepository<LoanRequest>
{
    Task<List<LoanRequestDto>> GetMyLoansAsync(string borrowerId);
    Task<LoanRequest> GetByIdWithBorrowerAsync(string id);
}
