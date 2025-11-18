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
            // 1. Migrate Application Context (Loans, AppUsers, etc.)
            logger.LogInformation("Applying ApplicationDb migrations...");
            ApplicationDbContext appDbContext = services.GetRequiredService<ApplicationDbContext>();
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("ApplicationDb migrations applied.");

            // 2. Migrate Identity Context (AspNetUsers, Roles)
            logger.LogInformation("Applying IdentityDb migrations...");
            AppIdentityDbContext identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            await identityDbContext.Database.MigrateAsync();
            logger.LogInformation("IdentityDb migrations applied.");

            // 3. Seed Identity Data (Users & Roles)
            logger.LogInformation("Seeding Identity data...");
            await IdentityDataSeeder.SeedRolesAndUsersAsync(services);
            logger.LogInformation("Identity seeding complete.");

            // 4. Seed Application Data (Loans, History, Payments)
            logger.LogInformation("Seeding Application data...");
            await ApplicationDataSeeder.SeedSampleDataAsync(services);
            logger.LogInformation("Application data seeding complete.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during startup migrations or seeding.");
            throw;
        }
    }
}
