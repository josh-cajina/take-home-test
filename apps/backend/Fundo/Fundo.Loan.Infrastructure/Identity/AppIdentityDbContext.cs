using Fundo.Loan.Infrastructure.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Infrastructure.Identity;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schemas.Identity);
    }
}
