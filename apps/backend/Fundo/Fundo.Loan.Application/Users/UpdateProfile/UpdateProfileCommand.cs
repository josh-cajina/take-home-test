using MediatR;

namespace Fundo.Loan.Application.Users.UpdateProfile;
public class UpdateProfileCommand : IRequest
{
    public Guid? AppUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
}
