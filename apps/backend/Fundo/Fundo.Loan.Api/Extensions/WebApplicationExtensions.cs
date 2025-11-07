using Fundo.Loan.Infrastructure.Identity;
using Fundo.Loan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        // We only run this in Development.
        if (!app.Services.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            return;
        }

        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying ApplicationDb migrations...");
            ApplicationDbContext appDbContext = services.GetRequiredService<ApplicationDbContext>();
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("ApplicationDb migrations applied.");

            logger.LogInformation("Applying IdentityDb migrations...");
            AppIdentityDbContext identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            await identityDbContext.Database.MigrateAsync();
            logger.LogInformation("IdentityDb migrations applied.");

            // --- SEEDING ---
            logger.LogInformation("Seeding initial data...");
            await IdentityDataSeeder.SeedRolesAndAdminAsync(services);
            logger.LogInformation("Seeding complete.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during startup migrations or seeding.");
            throw;
        }
    }
}
