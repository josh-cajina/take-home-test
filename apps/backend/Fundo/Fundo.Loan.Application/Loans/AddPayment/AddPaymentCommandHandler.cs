using FluentValidation;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Domain.Entities;
using MediatR;

namespace Fundo.Loan.Application.Loans.AddPayment;

public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddPaymentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        Guid processorId = await _currentUserService.GetAppUserIdAsync();
        LoanRequest? loan = await _context.LoanRequests.FindAsync([request.LoanId], cancellationToken);

        if (loan == null)
        {
            throw new Exception("Loan not found."); // Use custom NotFoundException
        }
        if (loan.LoanStatusId != 3) // 3 = "Approved"
        {
            throw new ValidationException("Payments can only be added to approved loans.");
        }
        if (request.Amount > loan.CurrentBalance)
        {
            throw new ValidationException("Payment amount cannot exceed the current balance.");
        }
        if (request.Amount <= 0)
        {
            throw new ValidationException("Payment amount must be positive.");
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            LoanId = request.LoanId,
            AmountPaid = request.Amount,
            PaymentDate = DateTime.UtcNow,
            ProcessedByAppUserId = processorId
        };

        // --- CRITICAL: Update the balance ---
        loan.CurrentBalance -= request.Amount;

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
