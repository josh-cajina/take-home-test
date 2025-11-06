using Fundo.Loan.Domain.Common;

namespace Fundo.Loan.Domain.Entities;

public sealed class LoanRequest : AuditableEntity
{
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public LoanStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? DecisionDate { get; set; }

    public string LoanBorrowerId { get; set; }
    public LoanBorrower LoanBorrower { get; set; }

    public string? LoanOfficerId { get; set; }
    public LoanOfficer? LoanOfficer { get; set; }
}
