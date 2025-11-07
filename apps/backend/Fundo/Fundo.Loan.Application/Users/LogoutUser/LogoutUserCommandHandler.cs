using Fundo.Loan.Application.Common.Interfaces;
using MediatR;

namespace Fundo.Loan.Application.Users.LogoutUser;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IIdentityService _identityService;

    public LogoutUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _identityService.LogoutUserAsync();
    }
}
