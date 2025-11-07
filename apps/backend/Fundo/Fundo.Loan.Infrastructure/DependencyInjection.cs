using System.Text;
using Fundo.Loan.Application.Common.Interfaces;
using Fundo.Loan.Application.Common.Settings;
using Fundo.Loan.Infrastructure.Common;
using Fundo.Loan.Infrastructure.Identity;
using Fundo.Loan.Infrastructure.Persistence;
using Fundo.Loan.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fundo.Loan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AdminUserOptions>(configuration.GetSection(AdminUserOptions.SectionName));

        // --- DbContexts ---
        // 1. Application DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application);
                }));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // 2. Identity DbContext
        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity);
                }));

        // --- Identity ---
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        // --- Authentication (JWT) ---
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            JwtSettings? jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings?.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // --- Services ---
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }
}
