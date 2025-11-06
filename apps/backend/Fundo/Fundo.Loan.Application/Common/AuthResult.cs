namespace Fundo.Loan.Application.Common;
public record AuthResult(bool Succeeded, string? Token = null, IEnumerable<string>? Errors = null);
