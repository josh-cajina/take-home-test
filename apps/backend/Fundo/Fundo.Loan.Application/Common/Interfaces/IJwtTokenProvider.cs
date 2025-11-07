namespace Fundo.Loan.Application.Common.Interfaces;

public interface IJwtTokenProvider
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles);
}
