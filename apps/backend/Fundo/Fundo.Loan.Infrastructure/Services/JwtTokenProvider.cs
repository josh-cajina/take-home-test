using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fundo.Loan.Infrastructure.Services;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenProvider(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(string userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
