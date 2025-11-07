namespace Fundo.Loan.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string IdentityId { get; }

    Guid AppUserId { get; }

    bool IsInRole(string roleName);

    Task<Guid> GetAppUserIdAsync();
}
