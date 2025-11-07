using Fundo.Loan.Application.Users.Common;
using MediatR;

namespace Fundo.Loan.Application.Users.LoginUser;

public class LoginUserCommand : IRequest<AuthResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
