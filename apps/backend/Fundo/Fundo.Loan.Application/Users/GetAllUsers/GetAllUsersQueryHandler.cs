using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Common.Models;
using Fundo.Loan.Application.Users.Dtos;
using Fundo.Loan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Loan.Application.Users.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService; // <-- NEW

    public GetAllUsersQueryHandler(
        IIdentityService identityService,
        IApplicationDbContext context,
        ICurrentUserService currentUserService) // <-- NEW
    {
        _identityService = identityService;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // 1. Get current user ID to exclude
        string currentIdentityId = _currentUserService.IdentityId;

        // 2. Get all Identity users
        List<IdentityUserDto> identityUsers = await _identityService.GetAllUsersAsync();

        // 3. Filter out the current user
        identityUsers = identityUsers.Where(u => u.Id != currentIdentityId).ToList();

        // 4. Get profiles
        List<AppUser> appUsers = await _context.AppUsers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var appUserMap = appUsers.ToDictionary(u => u.IdentityId, u => u);

        // 5. Map
        var userDtos = identityUsers.Select(identityUser =>
        {
            appUserMap.TryGetValue(identityUser.Id, out AppUser? appUser);

            return new UserDto
            {
                IdentityId = identityUser.Id,
                Email = identityUser.Email,
                Roles = identityUser.Roles,
                AppUserId = appUser?.Id ?? Guid.Empty,
                FirstName = appUser?.FirstName ?? "N/A",
                LastName = appUser?.LastName ?? "N/A"
            };
        }).ToList();

        return userDtos;
    }
}
