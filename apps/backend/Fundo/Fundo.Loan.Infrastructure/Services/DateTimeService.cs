using Fundo.Loan.Application.Common.Interfaces;

namespace Fundo.Loan.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
