using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fundo.Loan.Domain.Common;

namespace Fundo.Loan.Domain.Entities;

public sealed class LoanApplication : BaseEntity
{
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime SubmittedDate { get; set; }
    public LoanStatus Status { get; set; }
    public string MyProperty { get; set; }
}
