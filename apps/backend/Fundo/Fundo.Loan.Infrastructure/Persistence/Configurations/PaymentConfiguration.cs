using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.AmountPaid)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.HasOne(p => p.Loan)
            .WithMany(l => l.Payments)
            .HasForeignKey(p => p.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProcessedByUser)
            .WithMany()
            .HasForeignKey(p => p.ProcessedByAppUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
