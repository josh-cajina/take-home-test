using Fundo.Loan.Application.Common;
using Fundo.Loan.Application.DTOs;

namespace Fundo.Loan.Application.Abstractions;
public interface IIdentityService
{
    Task<AuthResult> RegisterBorrowerAsync(RegisterDto dto);
    Task<AuthResult> RegisterLoanOfficerAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
}
