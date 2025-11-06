using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fundo.Loan.Domain.Common;

namespace Fundo.Loan.Domain.Entities;
public sealed class Borrower : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string IdentityId { get; set; }
    public ICollection<LoanRequest> LoanRequests { get; set; } = [];
}
