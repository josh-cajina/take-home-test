namespace Fundo.Loan.Domain.Entities;

public class LoanRequest
{
    public Guid Id { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public int TermInMonths { get; set; }
    public string Purpose { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? DecisionDate { get; set; }

    public int LoanStatusId { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? AnalystId { get; set; }

    public virtual LoanStatus Status { get; set; }
    public virtual AppUser Requester { get; set; }
    public virtual AppUser Analyst { get; set; }
    public virtual ICollection<LoanHistory> History { get; set; } = new List<LoanHistory>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
