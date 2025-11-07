using Fundo.Loan.Application.Users.Common;
using MediatR;

namespace Fundo.Loan.Application.Users.RegisterUser;

public class RegisterUserCommand : IRequest<AuthResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
