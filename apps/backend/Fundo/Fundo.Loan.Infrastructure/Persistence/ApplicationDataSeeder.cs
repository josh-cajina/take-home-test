using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Common.Settings;
using Fundo.Loan.Domain.Entities;
using Fundo.Loan.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fundo.Loan.Infrastructure.Persistence;
public static class ApplicationDataSeeder
{
    public static async Task SeedSampleDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IApplicationDbContext appContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        AdminUserOptions adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserOptions>>().Value;

        Guid adminAppId = await GetOrCreateAppUserAsync(userManager, appContext, adminOptions.Email, "System", "Administrator");

        if (await appContext.LoanRequests.AnyAsync())
        {
            return;
        }

        Guid analyst1AppId = await GetOrCreateAppUserAsync(userManager, appContext, "analyst1@loanapp.com", "Alice", "Analyst");
        Guid analyst2AppId = await GetOrCreateAppUserAsync(userManager, appContext, "analyst2@loanapp.com", "Bob", "Reviewer");
        Guid requesterAppId = await GetOrCreateAppUserAsync(userManager, appContext, "requester1@loanapp.com", "John", "Requester");
        Guid requester2AppId = await GetOrCreateAppUserAsync(userManager, appContext, "requester2@loanapp.com", "Jane", "Client");
        Guid analystReqAppId = await GetOrCreateAppUserAsync(userManager, appContext, "analystreq@loanapp.com", "Diana", "Demo");

        if (adminAppId == Guid.Empty ||
            analyst1AppId == Guid.Empty ||
            analyst2AppId == Guid.Empty ||
            requesterAppId == Guid.Empty ||
            requester2AppId == Guid.Empty)
        {
            return;
        }

        var loans = new List<LoanRequest>
            {
                // -- Unassigned Loans --
                new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requesterAppId, AnalystId = null, LoanStatusId = 1, // Pending
                    RequestedAmount = 5000, CurrentBalance = 5000, TermInMonths = 12, Purpose = "Home Renovation", RequestedDate = DateTime.UtcNow.AddDays(-2)
                },
                new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requester2AppId, AnalystId = null, LoanStatusId = 1, // Pending
                    RequestedAmount = 1200, CurrentBalance = 1200, TermInMonths = 6, Purpose = "Emergency Medical", RequestedDate = DateTime.UtcNow.AddDays(-1)
                },

                // -- Assigned to Analyst 1 (Alice) --
                new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requesterAppId, AnalystId = analyst1AppId, LoanStatusId = 2, // Under Review
                    RequestedAmount = 15000, CurrentBalance = 15000, TermInMonths = 48, Purpose = "Car Purchase", RequestedDate = DateTime.UtcNow.AddDays(-5)
                },
                new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requester2AppId, AnalystId = analyst1AppId, LoanStatusId = 3, // Approved
                    RequestedAmount = 2000, CurrentBalance = 1500, TermInMonths = 12, Purpose = "Education", RequestedDate = DateTime.UtcNow.AddDays(-10), DecisionDate = DateTime.UtcNow.AddDays(-8)
                },
                 new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requesterAppId, AnalystId = analyst1AppId, LoanStatusId = 4, // Rejected
                    RequestedAmount = 50000, CurrentBalance = 50000, TermInMonths = 60, Purpose = "Business Startup", RequestedDate = DateTime.UtcNow.AddDays(-20), DecisionDate = DateTime.UtcNow.AddDays(-19)
                },

                // -- Assigned to Analyst 2 (Bob) --
                new LoanRequest {
                    Id = Guid.NewGuid(), RequesterId = requesterAppId, AnalystId = analyst2AppId, LoanStatusId = 2, // Under Review
                    RequestedAmount = 3500, CurrentBalance = 3500, TermInMonths = 24, Purpose = "Vacation", RequestedDate = DateTime.UtcNow.AddDays(-3)
                }
            };

        if (analystReqAppId != Guid.Empty)
        {
            loans.Add(new LoanRequest
            {
                Id = Guid.NewGuid(),
                RequesterId = analystReqAppId,
                AnalystId = null,
                LoanStatusId = 1,
                RequestedAmount = 2000.00m,
                CurrentBalance = 2000.00m,
                TermInMonths = 12,
                Purpose = "Analyst Self-Request Test",
                RequestedDate = DateTime.UtcNow.AddDays(-1)
            });
        }

        await appContext.LoanRequests.AddRangeAsync(loans);
        await appContext.SaveChangesAsync(default);

        // 5. Add Payments
        LoanRequest? approvedLoan = loans.FirstOrDefault(l => l.LoanStatusId == 3);
        if (approvedLoan != null)
        {
            var payments = new List<Payment>
                {
                    new Payment { Id = Guid.NewGuid(), LoanId = approvedLoan.Id, AmountPaid = 250, PaymentDate = DateTime.UtcNow.AddDays(-2), ProcessedByAppUserId = adminAppId },
                    new Payment { Id = Guid.NewGuid(), LoanId = approvedLoan.Id, AmountPaid = 250, PaymentDate = DateTime.UtcNow.AddDays(-1), ProcessedByAppUserId = analyst1AppId }
                };
            await appContext.Payments.AddRangeAsync(payments);
            await appContext.SaveChangesAsync(default);
        }
    }

    private static async Task<Guid> GetOrCreateAppUserAsync(UserManager<ApplicationUser> userManager, IApplicationDbContext appContext, string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Guid.Empty;
        }

        ApplicationUser? identityUser = await userManager.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return Guid.Empty;
        }

        AppUser? appUser = await appContext.AppUsers.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);

        if (appUser == null)
        {
            appUser = new AppUser
            {
                Id = Guid.NewGuid(),
                IdentityId = identityUser.Id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Address = "123 Seeded St"
            };
            await appContext.AppUsers.AddAsync(appUser);
            await appContext.SaveChangesAsync(default);
        }

        return appUser.Id;
    }
}
