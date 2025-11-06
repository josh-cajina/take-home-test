using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Data.Configurations;
public sealed class LoanRequestConfiguration : IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> builder)
    {
        builder
            .HasKey(request => request.Id);

        builder
            .Property(request => request.Amount)
            .HasColumnType("decimal(18,2)");

        builder
            .Property(request => request.Purpose)
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(request => request.Status)
            .IsRequired()
            .HasConversion<string>();

        builder
            .HasIndex(request => request.LoanBorrowerId);
        
        builder
            .HasIndex(request => request.LoanOfficerId);
        
        builder
            .HasIndex(request => request.Status);
    }
}
