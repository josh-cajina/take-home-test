using Asp.Versioning;
using Fundo.Loan.Api.Common;
using Fundo.Loan.Application;
using Fundo.Loan.Infrastructure;
using Fundo.Loan.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Fundo.Loan.Api;

public static class DependencyInjection
{
    public static readonly string AllowAngularAppPolicy = "AllowAngularApp";

    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder
            .Services
                .AddControllers(options =>
                {
                    options.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
                    = new CamelCasePropertyNamesContractResolver())
                .AddXmlSerializerFormatters();

        builder
            .Services
                .Configure<MvcOptions>(options =>
                {
                    NewtonsoftJsonOutputFormatter formatter = options.OutputFormatters
                        .OfType<NewtonsoftJsonOutputFormatter>()
                        .First();

                    formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV1);
                });

        builder
            .Services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1.0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionSelector = new DefaultApiVersionSelector(options);

                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new MediaTypeApiVersionReader());
                })
                .AddMvc();

        builder
            .Services
                .AddOpenApi();

        builder
            .Services
                .AddHttpContextAccessor();

        return builder;
    }

    public static WebApplicationBuilder AddCORS(this WebApplicationBuilder builder)
    {
        builder.
            Services
                .AddCors(options =>
                {
                    options.AddPolicy(name: AllowAngularAppPolicy,
                        policyBuilder =>
                        {
                            policyBuilder.WithOrigins("http://localhost:4200")
                                   .AllowAnyHeader()
                                   .AllowAnyMethod();
                        });
                });

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder
            .Services
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

        return builder;
    }

    public static WebApplicationBuilder AddDatabaseServices(this WebApplicationBuilder builder)
    {
        builder
            .Services
                .AddOrmServices(builder.Configuration)
                .AddRepositories();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder
            .Services
                .AddIdentityServices()
                .AddCQRS();

        return builder;
    }


}
