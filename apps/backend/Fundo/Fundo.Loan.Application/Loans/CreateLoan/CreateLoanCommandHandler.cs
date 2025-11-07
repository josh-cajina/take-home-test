using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Domain.Entities;
using MediatR;

namespace Fundo.Loan.Application.Loans.CreateLoan;

public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateLoanCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        Guid requesterId = await _currentUserService.GetAppUserIdAsync();
        if (requesterId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User profile not found.");
        }

        var loan = new LoanRequest
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            RequestedAmount = request.RequestedAmount,
            CurrentBalance = request.RequestedAmount,
            TermInMonths = request.TermInMonths,
            Purpose = request.Purpose,
            RequestedDate = DateTime.UtcNow,
            LoanStatusId = 1, // 1 = "Pending"
            AnalystId = null
        };

        var history = new LoanHistory
        {
            Id = Guid.NewGuid(),
            LoanId = loan.Id,
            ChangedByAppUserId = requesterId,
            OldStatusId = null,
            NewStatusId = 1,
            Timestamp = DateTime.UtcNow,
            Comment = "Loan request created."
        };

        _context.LoanRequests.Add(loan);
        _context.LoanHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);
        return loan.Id;
    }
}
