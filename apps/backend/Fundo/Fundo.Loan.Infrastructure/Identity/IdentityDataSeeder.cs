using Fundo.Loan.Application.Common.Settings;
using Fundo.Loan.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fundo.Loan.Infrastructure.Identity;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        AdminUserOptions adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserOptions>>().Value;

        // 1. Create Roles
        await SeedRoleAsync(roleManager, Roles.Admin);
        await SeedRoleAsync(roleManager, Roles.Analyst);
        await SeedRoleAsync(roleManager, Roles.Requester);

        // 2. Create Admin
        await SeedUserAsync(userManager, adminOptions.Email, adminOptions.Password, Roles.Admin);

        // 3. Create Analysts
        await SeedUserAsync(userManager, "analyst1@loanapp.com", "StrongPassword!123", Roles.Analyst);
        await SeedUserAsync(userManager, "analyst2@loanapp.com", "StrongPassword!123", Roles.Analyst);

        // 4. Create Requesters
        await SeedUserAsync(userManager, "requester1@loanapp.com", "StrongPassword!123", Roles.Requester);
        await SeedUserAsync(userManager, "requester2@loanapp.com", "StrongPassword!123", Roles.Requester);
    }

    private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
