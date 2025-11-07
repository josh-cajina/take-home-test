using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Persistence.Configurations;

public class LoanStatusConfiguration : IEntityTypeConfiguration<LoanStatus>
{
    public void Configure(EntityTypeBuilder<LoanStatus> builder)
    {
        builder.ToTable("LoanStatuses");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new LoanStatus { Id = 1, Name = "Pending" },
            new LoanStatus { Id = 2, Name = "Under Review" },
            new LoanStatus { Id = 3, Name = "Approved" },
            new LoanStatus { Id = 4, Name = "Rejected" }
        );
    }
}
