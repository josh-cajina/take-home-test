using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fundo.Loan.Domain.Common;

namespace Fundo.Loan.Domain.Entities;

public sealed class LoanRequest : AuditableEntity
{
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public LoanStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? DecisionDate { get; set; }

    public string BorrowerId { get; set; }
    public Borrower Borrower { get; set; }

    public string? LoanOfficerId { get; set; }
    public LoanOfficer? LoanOfficer { get; set; }
}
