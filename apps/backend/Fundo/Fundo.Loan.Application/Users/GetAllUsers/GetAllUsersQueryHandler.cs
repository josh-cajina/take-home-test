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

    public GetAllUsersQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // 1. Get all Identity users (Email, Roles)
        List<IdentityUserDto> identityUsers = await _identityService.GetAllUsersAsync();

        // 2. Get all AppUser profiles (FirstName, LastName)
        List<AppUser> appUsers = await _context.AppUsers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 3. Create a lookup for efficient joining
        var appUserMap = appUsers.ToDictionary(u => u.IdentityId, u => u);

        // 4. Join the two lists
        var userDtos = identityUsers.Select(identityUser =>
        {
            // Find the matching profile
            appUserMap.TryGetValue(identityUser.Id, out AppUser appUser);

            return new UserDto
            {
                IdentityId = identityUser.Id,
                Email = identityUser.Email,
                Roles = identityUser.Roles,
                // Handle cases where an Identity user might not have an AppUser profile yet
                AppUserId = appUser?.Id ?? Guid.Empty,
                FirstName = appUser?.FirstName ?? "N/A",
                LastName = appUser?.LastName ?? "N/A"
            };
        }).ToList();

        return userDtos;
    }
}
