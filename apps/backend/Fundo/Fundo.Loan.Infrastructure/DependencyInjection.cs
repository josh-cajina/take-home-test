using Fundo.Loan.Application.Abstractions;
using Fundo.Loan.Infrastructure.Data;
using Fundo.Loan.Infrastructure.Data.Common;
using Fundo.Loan.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Loan.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddOrmServices(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(
                        connectionString,
                        builder =>
                        {
                            builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                            builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application);
                        }));

        services
            .AddDbContext<ApplicationIdentityDbContext>(options =>
                options
                    .UseSqlServer(
                        connectionString,
                        builder =>
                        {
                            builder.MigrationsAssembly(typeof(ApplicationIdentityDbContext).Assembly.FullName);
                            builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity);
                        }));

        return services;
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>();

        services
            .AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services
            .AddScoped<ILoanBorrowerRepository, LoanBorrowerRepository>();

        services
            .AddScoped<ILoanOfficerRepository, LoanOfficerRepository>();

        return services;
    }
}
