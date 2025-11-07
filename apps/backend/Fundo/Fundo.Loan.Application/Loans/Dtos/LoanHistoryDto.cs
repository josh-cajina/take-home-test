namespace Fundo.Loan.Application.Loans.Dtos;

public class LoanHistoryDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public DateTime Timestamp { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
}
