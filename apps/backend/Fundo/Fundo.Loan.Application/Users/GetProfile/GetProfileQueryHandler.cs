using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Users.Dtos;
using Fundo.Loan.Domain.Constants;
using Fundo.Loan.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Fundo.Loan.Application.Users.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetProfileQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        Guid currentUserId = await _currentUserService.GetAppUserIdAsync();
        Guid targetUserId = currentUserId;

        if (request.AppUserId.HasValue && request.AppUserId.Value != Guid.Empty)
        {
            if (request.AppUserId.Value != currentUserId && !_currentUserService.IsInRole(Roles.Admin))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this profile.");
            }
            targetUserId = request.AppUserId.Value;
        }

        AppUser? appUser = await _context.AppUsers.FindAsync([targetUserId], cancellationToken);

        if (appUser == null)
        {
            throw new Exception("Profile not found");
        }

        string? email = await _identityService.GetEmailAsync(appUser.IdentityId);

        return new UserProfileDto
        {
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Address = appUser.Address,
            DateOfBirth = appUser.DateOfBirth,
            Email = email ?? "N/A"
        };
    }
}
