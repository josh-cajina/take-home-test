using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Domain.Constants;
using Fundo.Loan.Domain.Entities;
using MediatR;

namespace Fundo.Loan.Application.Users.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        Guid currentUserId = await _currentUserService.GetAppUserIdAsync();
        Guid targetUserId = currentUserId;

        if (request.AppUserId.HasValue && request.AppUserId.Value != Guid.Empty)
        {
            if (request.AppUserId.Value != currentUserId && !_currentUserService.IsInRole(Roles.Admin))
            {
                throw new UnauthorizedAccessException("You are not authorized to edit this profile.");
            }
            targetUserId = request.AppUserId.Value;
        }

        AppUser? appUser = await _context.AppUsers.FindAsync([targetUserId], cancellationToken);

        if (appUser == null)
        {
            throw new Exception("User profile not found");
        }

        appUser.FirstName = request.FirstName;
        appUser.LastName = request.LastName;
        appUser.Address = request.Address;
        appUser.DateOfBirth = request.DateOfBirth;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
