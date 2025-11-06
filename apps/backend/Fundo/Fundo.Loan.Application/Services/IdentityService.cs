using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fundo.Loan.Application.Abstractions;
using Fundo.Loan.Application.Common;
using Fundo.Loan.Application.DTOs;
using Fundo.Loan.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Fundo.Loan.Application.Services;

public class IdentityService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    ILoanBorrowerRepository loanBorrowerRepository,
    ILoanOfficerRepository loanOfficerRepository,
    IUnitOfWork _unitOfWork) : IIdentityService
{
    public async Task<AuthResult> RegisterBorrowerAsync(RegisterDto dto)
    {
        var identityUser = new IdentityUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        IdentityResult identityResult = await userManager.CreateAsync(identityUser, dto.Password);

        if (!identityResult.Succeeded)
        {
            return new AuthResult(false, Errors: identityResult.Errors.Select(error => error.Description));
        }

        await userManager.AddToRoleAsync(identityUser, "Borrower");

        var borrower = new LoanBorrower
        {
            Id = $"b_{Guid.CreateVersion7()}",
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        try
        {
            loanBorrowerRepository.Add(borrower);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await userManager.DeleteAsync(identityUser);
            return new AuthResult(false, Errors: ["An error occurred creating the user profile.", ex.Message]);
        }

        return await LoginAsync(new LoginDto(dto.UserName, dto.Password));
    }

    public async Task<AuthResult> RegisterLoanOfficerAsync(RegisterDto dto)
    {
        var identityUser = new IdentityUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        IdentityResult identityResult = await userManager.CreateAsync(identityUser, dto.Password);

        if (!identityResult.Succeeded)
        {
            return new AuthResult(false, Errors: identityResult.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(identityUser, "LoanOfficer");

        var loanOfficer = new LoanOfficer
        {
            Id = $"o_{Guid.CreateVersion7()}",
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        try
        {
            loanOfficerRepository.Add(loanOfficer);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await userManager.DeleteAsync(identityUser);
            return new AuthResult(false, Errors: ["An error occurred creating the user profile.", ex.Message]);
        }

        return new AuthResult(true);
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        SignInResult result = await signInManager.PasswordSignInAsync(dto.UserName, dto.Password, false, false);

        if (!result.Succeeded)
        {
            return new AuthResult(false, Errors: ["Invalid username or password."]);
        }

        IdentityUser? user = await userManager.FindByNameAsync(dto.UserName);

        if (user == null)
        {
            return new AuthResult(false, Errors: ["Invalid username or password."]);
        }

        string token = await GenerateJwtToken(user);

        return new AuthResult(true, Token: token);
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes("secret");

        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new (ClaimTypes.Name, user.UserName ?? "")
            };

        IList<string> roles = await userManager.GetRolesAsync(user);

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "issuer",
            Audience = "audience",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
