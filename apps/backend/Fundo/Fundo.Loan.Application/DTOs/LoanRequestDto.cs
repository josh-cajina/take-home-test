namespace Fundo.Loan.Application.DTOs;
public record LoanRequestDto(Guid Id, decimal Amount, string Purpose, string Status, DateTime RequestedDate);
