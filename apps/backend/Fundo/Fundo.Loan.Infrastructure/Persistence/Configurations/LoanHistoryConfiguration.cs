using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Persistence.Configurations;

public class LoanHistoryConfiguration : IEntityTypeConfiguration<LoanHistory>
{
    public void Configure(EntityTypeBuilder<LoanHistory> builder)
    {
        builder.ToTable("LoanHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Timestamp)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(h => h.Loan)
            .WithMany(l => l.History)
            .HasForeignKey(h => h.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany(u => u.ChangesMade)
            .HasForeignKey(h => h.ChangedByAppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.OldStatus)
            .WithMany(s => s.HistoryAsOldStatus)
            .HasForeignKey(h => h.OldStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.NewStatus)
            .WithMany(s => s.HistoryAsNewStatus)
            .HasForeignKey(h => h.NewStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
