using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Loan.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An internal server error occurred."
        };

        // Only include detailed error information in Development
        if (_environment.IsDevelopment())
        {
            problemDetails.Detail = exception.ToString();
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}
