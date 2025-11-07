namespace Fundo.Loan.Application.Common.Models;

public class IdentityUserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
}
