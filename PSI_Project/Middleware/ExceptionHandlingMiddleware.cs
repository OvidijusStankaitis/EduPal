using Newtonsoft.Json;

namespace PSI_Project.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
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
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 400; // BadRequest

        var errorResponse = new
        {
            Message = "An unexpected error occurred.",
            ExceptionMessage = exception.Message,
            StackTrace = exception.StackTrace
        };

        var jsonResponse = JsonConvert.SerializeObject(errorResponse);

        return context.Response.WriteAsync(jsonResponse); // Write the JSON response to the response stream
    }
}