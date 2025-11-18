using Fundo.Loan.Application.Users.Dtos;
using MediatR;

namespace Fundo.Loan.Application.Users.GetProfile;

public class GetProfileQuery : IRequest<UserProfileDto>
{
    public Guid? AppUserId { get; set; }
}
