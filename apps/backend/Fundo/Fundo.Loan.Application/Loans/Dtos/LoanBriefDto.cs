namespace Fundo.Loan.Application.Loans.Dtos;

public class LoanBriefDto
{
    public Guid Id { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public string StatusName { get; set; }
    public DateTime RequestedDate { get; set; }
    public string RequesterFullName { get; set; }
    public string AnalystFullName { get; set; }
}
