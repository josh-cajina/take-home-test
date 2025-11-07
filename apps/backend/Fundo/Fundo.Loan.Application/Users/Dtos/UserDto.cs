namespace Fundo.Loan.Application.Users.Dtos;

public class UserDto
{
    public string IdentityId { get; set; }
    public Guid AppUserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IList<string> Roles { get; set; }
}
