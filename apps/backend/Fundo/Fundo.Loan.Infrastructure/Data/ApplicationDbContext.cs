using Fundo.Loan.Domain.Entities;
using Fundo.Loan.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<LoanBorrower> LoanBorrowers { get; set; }
    public DbSet<LoanOfficer> LoanOfficers { get; set; }
    public DbSet<LoanRequest> LoanRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
