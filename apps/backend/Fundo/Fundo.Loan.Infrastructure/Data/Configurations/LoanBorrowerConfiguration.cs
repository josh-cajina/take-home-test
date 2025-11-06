using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Data.Configurations;
public sealed class LoanBorrowerConfiguration : IEntityTypeConfiguration<LoanBorrower>
{
    public void Configure(EntityTypeBuilder<LoanBorrower> builder)
    {
        builder
            .HasKey(borrower => borrower.Id);

        builder
            .Property(borrower => borrower.Id)
            .HasMaxLength(500);

        builder
            .Property(borrower => borrower.FirstName)
            .HasMaxLength(50);

        builder
            .Property(borrower => borrower.LastName)
            .HasMaxLength(50);

        builder
            .HasMany(borrower => borrower.Requests)
            .WithOne(request => request.LoanBorrower)
            .HasForeignKey(request => request.LoanBorrowerId);
    }
}
