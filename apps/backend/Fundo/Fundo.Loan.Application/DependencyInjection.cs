using System.Reflection;
using FluentValidation;
using Fundo.Loan.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Loan.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register MediatR pipeline behavior for validation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
