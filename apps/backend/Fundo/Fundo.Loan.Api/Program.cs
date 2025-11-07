using Fundo.Loan.Api;
using Fundo.Loan.Api.Extensions;
using Fundo.Loan.Application;
using Fundo.Loan.Infrastructure;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAngularApp",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services
    .AddApi()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

await app.ApplyMigrationsAndSeedAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
