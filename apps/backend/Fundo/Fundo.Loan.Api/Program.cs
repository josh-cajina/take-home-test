using Fundo.Loan.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddDatabaseServices()
    .AddApiServices()
    .AddCORS()
    .AddApplicationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
