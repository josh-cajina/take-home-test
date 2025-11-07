using System.Reflection;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Domain.Entities;
using Fundo.Loan.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<LoanRequest> LoanRequests { get; set; }
    public DbSet<LoanStatus> LoanStatuses { get; set; }
    public DbSet<LoanHistory> LoanHistories { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
