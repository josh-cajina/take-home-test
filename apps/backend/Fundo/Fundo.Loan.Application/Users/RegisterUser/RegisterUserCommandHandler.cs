using FluentValidation;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Users.Common;
using Fundo.Loan.Domain.Constants;
using Fundo.Loan.Domain.Entities;
using MediatR;

namespace Fundo.Loan.Application.Users.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _appContext;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext appContext,
        IJwtTokenProvider jwtTokenProvider)
    {
        _identityService = identityService;
        _appContext = appContext;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the Identity login
        AuthResult authResult = await _identityService.RegisterUserAsync(request.Email, request.Password);

        if (!authResult.Succeeded)
        {
            throw new ValidationException(string.Join("\n", authResult.Errors));
        }

        // 2. Create the AppUser profile
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            IdentityId = authResult.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        _appContext.AppUsers.Add(appUser);
        await _appContext.SaveChangesAsync(cancellationToken);

        // 3. Assign Role
        string targetRole = string.IsNullOrWhiteSpace(request.Role) ? Roles.Requester : request.Role;

        if (targetRole.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            targetRole = Roles.Requester;
        }

        if (await _identityService.RoleExistsAsync(targetRole))
        {
            await _identityService.AddUserToRoleAsync(authResult.UserId, targetRole);
        }

        // 4. Generate JWT token
        IList<string> roles = await _identityService.GetUserRolesAsync(authResult.UserId);
        string token = _jwtTokenProvider.GenerateToken(authResult.UserId, request.Email, roles);

        return new AuthResponse
        {
            UserId = authResult.UserId,
            Email = request.Email,
            Token = token
        };
    }
}
