using System.Transactions;
using FluentValidation;
using FluentValidation.Results;
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
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        AuthResult authResult = await _identityService.RegisterUserAsync(request.Email, request.Password);

        if (!authResult.Succeeded)
        {
            IEnumerable<ValidationFailure> failures = authResult.Errors
                .Select(errorMsg => new ValidationFailure("Identity", errorMsg));

            throw new ValidationException(failures);
        }

        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            IdentityId = authResult.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            Address = "Not Provided"
        };

        _appContext.AppUsers.Add(appUser);
        await _appContext.SaveChangesAsync(cancellationToken);

        string targetRole = string.IsNullOrWhiteSpace(request.Role) ? Roles.Requester : request.Role;

        if (targetRole.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            targetRole = Roles.Requester;
        }

        if (await _identityService.RoleExistsAsync(targetRole))
        {
            await _identityService.AddUserToRoleAsync(authResult.UserId, targetRole);
        }

        IList<string> roles = await _identityService.GetUserRolesAsync(authResult.UserId);
        string token = _jwtTokenProvider.GenerateToken(authResult.UserId, request.Email, roles);

        scope.Complete();

        return new AuthResponse
        {
            UserId = authResult.UserId,
            Email = request.Email,
            Token = token
        };
    }
}
