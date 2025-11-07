using Fundo.Loan.Application.Common.Models;

namespace Fundo.Loan.Application.Common.Interfaces;

public record AuthResult(bool Succeeded, string UserId, string[] Errors);

public interface IIdentityService
{
    Task<AuthResult> RegisterUserAsync(string email, string password);
    Task<AuthResult> LoginUserAsync(string email, string password);
    Task LogoutUserAsync();

    Task<bool> RoleExistsAsync(string roleName);
    Task CreateRoleAsync(string roleName);
    Task AddUserToRoleAsync(string userId, string roleName);
    Task<IList<string>> GetUserRolesAsync(string userId);
    Task<List<IdentityUserDto>> GetAllUsersAsync();
}
