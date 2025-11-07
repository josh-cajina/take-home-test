using FluentValidation;
using Fundo.Loan.Application.Common.Interfaces;
using MediatR;

namespace Fundo.Loan.Application.Loans.UpdateLoanStatus;

public class UpdateLoanStatusCommandHandler : IRequestHandler<UpdateLoanStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateLoanStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateLoanStatusCommand request, CancellationToken cancellationToken)
    {
        Guid analystId = await _currentUserService.GetAppUserIdAsync();
        if (analystId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Analyst profile not found.");
        }

        Domain.Entities.LoanRequest? loan = await _context.LoanRequests.FindAsync([request.LoanId], cancellationToken);
        if (loan == null)
        {
            throw new Exception($"Loan with ID {request.LoanId} not found."); // Use custom NotFoundException
        }

        // --- GOVERNANCE RULE 1: Can't process your own loan ---
        if (loan.RequesterId == analystId)
        {
            throw new Exception("Analysts cannot process their own loan requests."); // Use custom ForbiddenException
        }

        // --- GOVERNANCE RULE 2: Claiming an unassigned loan ---
        if (loan.AnalystId == null && request.NewStatusId == 2) // 2 = "Under Review"
        {
            loan.AnalystId = analystId;
        }
        // --- GOVERNANCE RULE 3: Can't touch another analyst's assigned loan ---
        else if (loan.AnalystId != null && loan.AnalystId != analystId)
        {
            throw new Exception("This loan is assigned to another analyst."); // Use custom ForbiddenException
        }

        int oldStatusId = loan.LoanStatusId;

        // Workflow logic
        if (string.IsNullOrWhiteSpace(request.Comment))
        {
            throw new ValidationException("A comment is required to update loan status.");
        }

        loan.LoanStatusId = request.NewStatusId;

        if (request.NewStatusId == 3 || request.NewStatusId == 4) // 3="Approved", 4="Rejected"
        {
            loan.DecisionDate = DateTime.UtcNow;
        }

        var history = new Domain.Entities.LoanHistory
        {
            Id = Guid.NewGuid(),
            LoanId = loan.Id,
            ChangedByAppUserId = analystId,
            OldStatusId = oldStatusId,
            NewStatusId = request.NewStatusId,
            Timestamp = DateTime.UtcNow,
            Comment = request.Comment
        };

        _context.LoanHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
