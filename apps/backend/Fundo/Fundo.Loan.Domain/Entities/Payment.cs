namespace Fundo.Loan.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }

    public Guid LoanId { get; set; }

    public Guid ProcessedByAppUserId { get; set; }

    public virtual LoanRequest Loan { get; set; }
    public virtual AppUser ProcessedByUser { get; set; }
}
