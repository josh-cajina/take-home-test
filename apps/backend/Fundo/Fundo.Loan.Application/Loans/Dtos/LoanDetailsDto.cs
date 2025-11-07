namespace Fundo.Loan.Application.Loans.Dtos;

public class LoanDetailsDto
{
    public Guid Id { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public int TermInMonths { get; set; }
    public string Purpose { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? DecisionDate { get; set; }

    public string StatusName { get; set; }
    public string RequesterFullName { get; set; }
    public string AnalystFullName { get; set; }

    public List<LoanHistoryDto> History { get; set; }
    public List<PaymentDto> Payments { get; set; }
}
