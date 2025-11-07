using Fundo.Loan.Application.Users.Dtos;
using MediatR;

namespace Fundo.Loan.Application.Users.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}
