using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fundo.Loan.Api.OpenApi;

public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // --- 1. Create a document for each API version ---
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            string apiVersion = description.ApiVersion.ToString();
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"LoanApp API (OpenAPI 3.0) {description.GroupName}",
                Version = apiVersion,
                Description = description.IsDeprecated ? "This API version has been deprecated." : string.Empty,
            });
        }

        // --- 2. Add JWT "Authorize" button ---
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
    }
}
