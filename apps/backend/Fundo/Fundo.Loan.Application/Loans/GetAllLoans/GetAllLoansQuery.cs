using Fundo.Loan.Application.Loans.Dtos;
using MediatR;

namespace Fundo.Loan.Application.Loans.GetAllLoans;

public class GetAllLoansQuery : IRequest<List<LoanBriefDto>>
{
}
