using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Loans.Dtos;
using Fundo.Loan.Domain.Constants;
using Fundo.Loan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Application.Loans.GetAllLoans;

public class GetAllLoansQueryHandler : IRequestHandler<GetAllLoansQuery, List<LoanBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetAllLoansQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<LoanBriefDto>> Handle(GetAllLoansQuery request, CancellationToken cancellationToken)
    {
        IQueryable<LoanRequest> query = _context.LoanRequests
            .Include(loanRequest => loanRequest.Status)
            .Include(loanRequest => loanRequest.Requester)
            .Include(loanRequest => loanRequest.Analyst)
            .AsNoTracking();

        // --- GOVERNANCE RULES ---
        if (_currentUserService.IsInRole(Roles.Admin))
        {
            // Admin sees all, no filter.
        }
        else if (_currentUserService.IsInRole(Roles.Analyst))
        {
            Guid analystId = await _currentUserService.GetAppUserIdAsync();
            // Analyst sees UNASSIGNED loans OR loans ASSIGNED TO THEM.
            query = query.Where(l => l.AnalystId == null || l.AnalystId == analystId);
        }
        else if (_currentUserService.IsInRole(Roles.Requester))
        {
            Guid requesterId = await _currentUserService.GetAppUserIdAsync();
            // Requester sees ONLY THEIR loans.
            query = query.Where(l => l.RequesterId == requesterId);
        }
        else
        {
            // No role? No loans.
            return new List<LoanBriefDto>();
        }

        // Project to DTO (Manual example, use AutoMapper)
        return await query
            .Select(loanBriefDto => new LoanBriefDto
            {
                Id = loanBriefDto.Id,
                RequestedAmount = loanBriefDto.RequestedAmount,
                CurrentBalance = loanBriefDto.CurrentBalance,
                StatusName = loanBriefDto.Status.Name,
                RequestedDate = loanBriefDto.RequestedDate,
                RequesterFullName = loanBriefDto.Requester.FirstName + " " + loanBriefDto.Requester.LastName,
                AnalystFullName = loanBriefDto.AnalystId == null ? "Unassigned" : (loanBriefDto.Analyst.FirstName + " " + loanBriefDto.Analyst.LastName)
            })
            .ToListAsync(cancellationToken);
    }
}
