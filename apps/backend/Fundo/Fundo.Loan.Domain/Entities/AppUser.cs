namespace Fundo.Loan.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }

    public string IdentityId { get; set; }

    public virtual ICollection<LoanRequest> LoansAsRequester { get; set; } = new List<LoanRequest>();
    public virtual ICollection<LoanRequest> LoansAsAnalyst { get; set; } = new List<LoanRequest>();
    public virtual ICollection<LoanHistory> ChangesMade { get; set; } = new List<LoanHistory>();
}
