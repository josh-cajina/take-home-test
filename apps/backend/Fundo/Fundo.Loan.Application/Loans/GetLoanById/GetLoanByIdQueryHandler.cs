using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Loans.Dtos;
using Fundo.Loan.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Application.Loans.GetLoanById;

public class GetLoanByIdQueryHandler : IRequestHandler<GetLoanByIdQuery, LoanDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetLoanByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<LoanDetailsDto> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.LoanRequest? loan = await _context.LoanRequests.AsNoTracking()
            .Include(loanRequest => loanRequest.Status)
            .Include(loanRequest => loanRequest.Requester)
            .Include(loanRequest => loanRequest.Analyst)
            .Include(loanRequest => loanRequest.History).ThenInclude(history => history.OldStatus)
            .Include(loanRequest => loanRequest.History).ThenInclude(history => history.NewStatus)
            .Include(loanRequest => loanRequest.Payments).ThenInclude(payment => payment.ProcessedByUser)
            .FirstOrDefaultAsync(loanRequest => loanRequest.Id == request.Id, cancellationToken);

        if (loan == null)
        {
            throw new Exception("Loan not found."); // Use custom NotFoundException
        }

        // --- GOVERNANCE CHECK ---
        Guid appUserId = await _currentUserService.GetAppUserIdAsync();
        bool hasAccess = false;

        if (_currentUserService.IsInRole(Roles.Admin))
        {
            hasAccess = true;
        }

        if (loan.RequesterId == appUserId)
        {
            hasAccess = true;
        }

        if (_currentUserService.IsInRole(Roles.Analyst) && (loan.AnalystId == null || loan.AnalystId == appUserId))
        {
            hasAccess = true;
        }

        if (!hasAccess)
        {
            throw new Exception("You do not have permission to view this loan."); // Use custom ForbiddenException
        }

        // --- Manual Mapping (Use AutoMapper in production) ---
        return new LoanDetailsDto
        {
            Id = loan.Id,
            RequestedAmount = loan.RequestedAmount,
            CurrentBalance = loan.CurrentBalance,
            TermInMonths = loan.TermInMonths,
            Purpose = loan.Purpose,
            RequestedDate = loan.RequestedDate,
            DecisionDate = loan.DecisionDate,
            StatusName = loan.Status.Name,
            RequesterFullName = $"{loan.Requester.FirstName} {loan.Requester.LastName}",
            AnalystFullName = loan.Analyst != null ? $"{loan.Analyst.FirstName} {loan.Analyst.LastName}" : "Unassigned",

            History = [.. loan.History.Select(hist => new LoanHistoryDto
            {
                Id = hist.Id,
                Comment = hist.Comment,
                Timestamp = hist.Timestamp,
                OldStatus = hist.OldStatus.Name,
                NewStatus = hist.NewStatus.Name
            })],

            Payments = loan.Payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                AmountPaid = payment.AmountPaid,
                PaymentDate = payment.PaymentDate,
                ProcessedBy = $"{payment.ProcessedByUser.FirstName} {payment.ProcessedByUser.LastName}"
            }).ToList()
        };
    }
}
