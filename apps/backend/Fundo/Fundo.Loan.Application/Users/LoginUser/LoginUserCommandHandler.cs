using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Users.Common;
using MediatR;

namespace Fundo.Loan.Application.Users.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public LoginUserCommandHandler(IIdentityService identityService, IJwtTokenProvider jwtTokenProvider)
    {
        _identityService = identityService;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        AuthResult authResult = await _identityService.LoginUserAsync(request.Email, request.Password);
        if (!authResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Fetch all roles for the user
        IList<string> roles = await _identityService.GetUserRolesAsync(authResult.UserId);

        // Pass roles to the token generator
        string token = _jwtTokenProvider.GenerateToken(authResult.UserId, request.Email, roles);

        return new AuthResponse
        {
            UserId = authResult.UserId,
            Email = request.Email,
            Token = token
        };
    }
}
