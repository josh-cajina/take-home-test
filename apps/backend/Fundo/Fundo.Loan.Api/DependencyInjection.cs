using Fundo.Loan.Api.Exceptions;
using Fundo.Loan.Api.OpenApi;

namespace Fundo.Loan.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers().AddXmlDataContractSerializerFormatters();

        services.AddProblemDetails();

        // Register Exception Handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // --- API VERSIONING ---
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // --- OPENAPI (SWAGGER) ---
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
        });

        services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        services.AddAuthorization();
        return services;
    }
}
