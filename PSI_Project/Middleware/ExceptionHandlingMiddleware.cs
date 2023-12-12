using System.Net;
using System.Text.Json;

namespace PSI_Project.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;

        if (exception is InvalidOperationException)
        {
            code = HttpStatusCode.BadRequest;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var errorResponse = new
        {
            Message = "An unexpected error occurred.",
            ExceptionMessage = exception.Message,
            StackTrace = exception.StackTrace
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse);

        return context.Response.WriteAsync(jsonResponse);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}