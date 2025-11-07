namespace Fundo.Loan.Application.Loans.Dtos;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public string ProcessedBy { get; set; }
}
