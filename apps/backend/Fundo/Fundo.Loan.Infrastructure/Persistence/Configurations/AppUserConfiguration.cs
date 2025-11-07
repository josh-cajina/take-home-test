using Fundo.Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Loan.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.IdentityId).IsRequired();
        builder.HasIndex(u => u.IdentityId).IsUnique();
    }
}
