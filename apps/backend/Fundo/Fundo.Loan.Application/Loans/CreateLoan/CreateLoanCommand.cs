using MediatR;

namespace Fundo.Loan.Application.Loans.CreateLoan;
public class CreateLoanCommand : IRequest<Guid>
{
    public decimal RequestedAmount { get; set; }
    public int TermInMonths { get; set; }
    public string Purpose { get; set; }
}
