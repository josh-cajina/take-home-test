using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Data.Configurations;
public sealed class LoanOfficerConfiguration : IEntityTypeConfiguration<LoanOfficer>
{
    public void Configure(EntityTypeBuilder<LoanOfficer> builder)
    {
        builder
            .HasKey(officer => officer.Id);

        builder
            .Property(officer => officer.Id)
            .HasMaxLength(500);

        builder
            .Property(officer => officer.FirstName)
            .HasMaxLength(50);

        builder
            .Property(officer => officer.LastName)
            .HasMaxLength(50);

        builder
            .HasMany(officer => officer.Requests)
            .WithOne(request => request.LoanOfficer)
            .HasForeignKey(request => request.LoanOfficerId);
    }
}
