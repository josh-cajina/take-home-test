using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Loans.Dtos;
using Fundo.Loan.Domain.Constants;
using Fundo.Loan.Domain.Entities;
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
        LoanRequest? loan = await _context.LoanRequests.AsNoTracking()
            .Include(l => l.Status)
            .Include(l => l.Requester)
            .Include(l => l.Analyst)
            .Include(l => l.History).ThenInclude(h => h.OldStatus)
            .Include(l => l.History).ThenInclude(h => h.NewStatus)
            .Include(l => l.Payments).ThenInclude(p => p.ProcessedByUser)
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (loan == null)
        {
            throw new Exception("Loan not found.");
        }

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
            throw new Exception("You do not have permission to view this loan.");
        }

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

            History = loan.History.Select(hist => new LoanHistoryDto
            {
                Id = hist.Id,
                Comment = hist.Comment,
                Timestamp = hist.Timestamp,
                // FIX IS HERE: Use ?.Name to safely handle nulls
                OldStatus = hist.OldStatus?.Name ?? "N/A",
                NewStatus = hist.NewStatus.Name
            }).ToList(),

            Payments = loan.Payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                AmountPaid = payment.AmountPaid,
                PaymentDate = payment.PaymentDate,
                // Safety check here too just in case
                ProcessedBy = payment.ProcessedByUser != null
                    ? $"{payment.ProcessedByUser.FirstName} {payment.ProcessedByUser.LastName}"
                    : "Unknown"
            }).ToList()
        };
    }
}
