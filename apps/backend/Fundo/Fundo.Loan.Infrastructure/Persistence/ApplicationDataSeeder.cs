using Fundo.Loan.Domain.Entities;
using Fundo.Loan.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Loan.Infrastructure.Persistence;
public static class ApplicationDataSeeder
{
    public static async Task SeedSampleDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Use concrete context to access all DbSets
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if loans exist
        if (await context.LoanRequests.AnyAsync())
        {
            return;
        }

        // 1. Get AppUser IDs (Create profiles if missing)
        Guid requester1Id = await EnsureAppUserAsync(context, userManager, "requester1@loanapp.com", "John", "Requester");
        Guid requester2Id = await EnsureAppUserAsync(context, userManager, "requester2@loanapp.com", "Jane", "Client");
        Guid analyst1Id = await EnsureAppUserAsync(context, userManager, "analyst1@loanapp.com", "Alice", "Analyst");
        Guid analyst2Id = await EnsureAppUserAsync(context, userManager, "analyst2@loanapp.com", "Bob", "Reviewer");

        if (requester1Id == Guid.Empty || analyst1Id == Guid.Empty)
        {
            return;
        }

        // 2. Create Loans
        var loans = new List<LoanRequest>
            {
                // -- Unassigned Loans --
                new()
                {
                    Id = Guid.NewGuid(), RequesterId = requester1Id, AnalystId = null, LoanStatusId = 1, // Pending
                    RequestedAmount = 5000, CurrentBalance = 5000, TermInMonths = 12, Purpose = "Home Renovation", RequestedDate = DateTime.UtcNow.AddDays(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(), RequesterId = requester2Id, AnalystId = null, LoanStatusId = 1, // Pending
                    RequestedAmount = 1200, CurrentBalance = 1200, TermInMonths = 6, Purpose = "Emergency Medical", RequestedDate = DateTime.UtcNow.AddDays(-1)
                },

                // -- Assigned to Analyst 1 (Alice) --
                new()
                {
                    Id = Guid.NewGuid(), RequesterId = requester1Id, AnalystId = analyst1Id, LoanStatusId = 2, // Under Review
                    RequestedAmount = 15000, CurrentBalance = 15000, TermInMonths = 48, Purpose = "Car Purchase", RequestedDate = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = Guid.NewGuid(), RequesterId = requester2Id, AnalystId = analyst1Id, LoanStatusId = 3, // Approved
                    RequestedAmount = 2000, CurrentBalance = 1500, TermInMonths = 12, Purpose = "Education", RequestedDate = DateTime.UtcNow.AddDays(-10), DecisionDate = DateTime.UtcNow.AddDays(-8)
                },
                 new()
                 {
                    Id = Guid.NewGuid(), RequesterId = requester1Id, AnalystId = analyst1Id, LoanStatusId = 4, // Rejected
                    RequestedAmount = 50000, CurrentBalance = 50000, TermInMonths = 60, Purpose = "Business Startup", RequestedDate = DateTime.UtcNow.AddDays(-20), DecisionDate = DateTime.UtcNow.AddDays(-19)
                },

                // -- Assigned to Analyst 2 (Bob) --
                new()
                {
                    Id = Guid.NewGuid(), RequesterId = requester1Id, AnalystId = analyst2Id, LoanStatusId = 2, // Under Review
                    RequestedAmount = 3500, CurrentBalance = 3500, TermInMonths = 24, Purpose = "Vacation", RequestedDate = DateTime.UtcNow.AddDays(-3)
                }
            };

        await context.LoanRequests.AddRangeAsync(loans);
        await context.SaveChangesAsync();

        // 3. Add Payments (For the approved loan)
        LoanRequest approvedLoan = loans.First(l => l.LoanStatusId == 3);
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            LoanId = approvedLoan.Id,
            AmountPaid = 500,
            PaymentDate = DateTime.UtcNow.AddDays(-1),
            ProcessedByAppUserId = analyst1Id
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();
    }

    private static async Task<Guid> EnsureAppUserAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        string email, string firstName, string lastName)
    {
        ApplicationUser? identityUser = await userManager.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return Guid.Empty;
        }

        AppUser? appUser = await context.AppUsers.FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);
        if (appUser == null)
        {
            appUser = new AppUser
            {
                Id = Guid.NewGuid(),
                IdentityId = identityUser.Id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Address = "123 Test St"
            };
            context.AppUsers.Add(appUser);
            await context.SaveChangesAsync();
        }
        return appUser.Id;
    }
}
