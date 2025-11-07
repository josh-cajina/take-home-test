namespace Fundo.Loan.Domain.Entities;

public class LoanHistory
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Comment { get; set; }
    
    public Guid LoanId { get; set; }
    public Guid ChangedByAppUserId { get; set; }
    public int? OldStatusId { get; set; }
    public int NewStatusId { get; set; }

    public virtual LoanRequest Loan { get; set; }
    public virtual AppUser ChangedByUser { get; set; }
    public virtual LoanStatus OldStatus { get; set; }
    public virtual LoanStatus NewStatus { get; set; }
}
