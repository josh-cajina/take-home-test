using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> AppUsers { get; }
    DbSet<LoanRequest> LoanRequests { get; }
    DbSet<LoanStatus> LoanStatuses { get; }
    DbSet<LoanHistory> LoanHistories { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
