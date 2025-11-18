using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Common.Models;
using Fundo.Loan.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<AuthResult> RegisterUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return new AuthResult(
            result.Succeeded,
            result.Succeeded ? user.Id : string.Empty,
            [.. result.Errors.Select(e => e.Description)]
        );
    }

    public async Task<AuthResult> LoginUserAsync(string email, string password)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return new AuthResult(false, string.Empty, ["Invalid email or password."]);
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        return new AuthResult(true, user?.Id!, []);
    }

    public async Task LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task CreateRoleAsync(string roleName)
    {
        if (!await RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    public async Task AddUserToRoleAsync(string userId, string roleName)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<List<IdentityUserDto>> GetAllUsersAsync()
    {
        List<ApplicationUser> users = await _userManager.Users.AsNoTracking().ToListAsync();
        var userDtos = new List<IdentityUserDto>();

        foreach (ApplicationUser user in users)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new IdentityUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            });
        }

        return userDtos;
    }

    public async Task<string?> GetEmailAsync(string userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }
}
