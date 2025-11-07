using Fundo.Loan.Application.Common.Settings;
using Fundo.Loan.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fundo.Loan.Infrastructure.Identity;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed Roles
        await SeedRoleAsync(roleManager, Roles.Admin);
        await SeedRoleAsync(roleManager, Roles.Analyst);
        await SeedRoleAsync(roleManager, Roles.Requester);

        // Seed Admin User
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        AdminUserOptions adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserOptions>>().Value;
        //ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // Using Program for context

        await SeedAdminUserAsync(userManager, adminOptions);
    }

    private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, AdminUserOptions options)
    {
        if (options == null || string.IsNullOrWhiteSpace(options.Email))
        {
            //logger.LogWarning("AdminUser settings are not configured. Skipping admin user creation.");
            return;
        }

        ApplicationUser? adminUser = await userManager.FindByEmailAsync(options.Email);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = options.Email,
                Email = options.Email,
                EmailConfirmed = true
            };

            IdentityResult result = await userManager.CreateAsync(adminUser, options.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                //logger.LogInformation("Admin user created successfully.");
            }
            else
            {
                //logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
