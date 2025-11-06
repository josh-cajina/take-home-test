using Fundo.Loan.Domain.Common;

namespace Fundo.Loan.Domain.Entities;
public sealed class LoanBorrower : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string IdentityId { get; set; }
    public ICollection<LoanRequest> Requests { get; set; } = [];
}
