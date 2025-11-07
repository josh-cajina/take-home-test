using Fundo.Loan.Application.Loans.Dtos;
using MediatR;

namespace Fundo.Loan.Application.Loans.GetLoanById;

public class GetLoanByIdQuery : IRequest<LoanDetailsDto>
{
    public Guid Id { get; set; }
}
