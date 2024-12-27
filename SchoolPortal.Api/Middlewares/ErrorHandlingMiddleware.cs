using FluentValidation;
using Newtonsoft.Json;

namespace SchoolPortal.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            ErrorCode = statusCode,
            Message = exception.Message,
            InnerException = exception.InnerException?.Message,
            Errors = exception is ValidationException validationException
            ? validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList()
            : GetExceptionMessages(exception)
        };

        var result = JsonConvert.SerializeObject(errorResponse);
        return context.Response.WriteAsync(result);
    }

    private class ErrorResponse
    {
        public int ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? InnerException { get; set; }
        public List<string>? Errors { get; set; }

    }

    private static List<string> GetExceptionMessages(Exception ex)
    {
        var errors = new List<string> { ex.Message };

        var innerException = ex.InnerException;
        while (innerException != null)
        {
            errors.Add(innerException.Message);
            innerException = innerException.InnerException;
        }

        return errors;
    }
}

