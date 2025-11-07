using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Loan.Api.Exceptions;

public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ValidationExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public ValidationExceptionHandler(
        ILogger<ValidationExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false; // This handler cannot process the exception.
        }

        _logger.LogWarning(exception, "A validation error occurred: {Message}", exception.Message);

        // Group errors by property name
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "A validation error occurred.",
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = errors }
            },
            Exception = exception
        });
    }
}
