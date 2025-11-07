using MediatR;

namespace Fundo.Loan.Application.Loans.UpdateLoanStatus;

public class UpdateLoanStatusCommand : IRequest
{
    public Guid LoanId { get; set; }
    public int NewStatusId { get; set; }
    public string Comment { get; set; }
}
