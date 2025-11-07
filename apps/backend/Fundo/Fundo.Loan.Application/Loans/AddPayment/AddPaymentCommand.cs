using MediatR;

namespace Fundo.Loan.Application.Loans.AddPayment;

public class AddPaymentCommand : IRequest<Guid>
{
    public Guid LoanId { get; set; }
    public decimal Amount { get; set; }
}
