using System.Net;
using System.Text.Json;
using ServiceMarketplace.Domain.Exceptions;

namespace ServiceMarketplace.API.Middleware;

/// <summary>
/// Global exception handler. Maps domain exceptions to HTTP status codes.
/// Unexpected exceptions return 500 without leaking stack traces.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/hangfire"))
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            UnauthorizedException e      => (HttpStatusCode.Forbidden, e.Message),
            SubscriptionLimitException e => (HttpStatusCode.PaymentRequired, e.Message),
            DomainException e            => (HttpStatusCode.BadRequest, e.Message),
            _                            => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var body = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(body);
    }
}
