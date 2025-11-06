using System.Reflection;
using FluentValidation;
using Fundo.Loan.Application.Abstractions;
using Fundo.Loan.Application.Pipelines;
using Fundo.Loan.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Loan.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
