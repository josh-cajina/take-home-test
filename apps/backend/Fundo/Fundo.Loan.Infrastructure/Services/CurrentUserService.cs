using System.Security.Claims;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    private AppUser _appUserCache; // Cache user lookup per request

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public string IdentityId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public Guid AppUserId => _appUserCache?.Id ?? Guid.Empty;

    public bool IsInRole(string roleName)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }

    public async Task<Guid> GetAppUserIdAsync()
    {
        if (_appUserCache != null)
        {
            return _appUserCache.Id;
        }

        if (string.IsNullOrEmpty(IdentityId))
        {
            return Guid.Empty;
        }

        _appUserCache = await _context.AppUsers.AsNoTracking().FirstAsync(u => u.IdentityId == IdentityId);

        return _appUserCache?.Id ?? Guid.Empty;
    }
}
