using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Persistence.Configurations;

public class LoanRequestConfiguration : IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.RequestedAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(l => l.CurrentBalance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(l => l.Purpose).IsRequired();

        builder.Property(l => l.RequestedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(l => l.Requester)
            .WithMany(u => u.LoansAsRequester)
            .HasForeignKey(l => l.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship to Analyst (AppUser)
        builder.HasOne(l => l.Analyst)
            .WithMany(u => u.LoansAsAnalyst)
            .HasForeignKey(l => l.AnalystId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship to LoanStatus
        builder.HasOne(l => l.Status)
            .WithMany(s => s.Loans)
            .HasForeignKey(l => l.LoanStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
