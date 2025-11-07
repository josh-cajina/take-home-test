namespace Fundo.Loan.Domain.Entities;

public class LoanStatus
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<LoanRequest> Loans { get; set; } = new List<LoanRequest>();

    public virtual ICollection<LoanHistory> HistoryAsOldStatus { get; set; } = new List<LoanHistory>();
    public virtual ICollection<LoanHistory> HistoryAsNewStatus { get; set; } = new List<LoanHistory>();
}
