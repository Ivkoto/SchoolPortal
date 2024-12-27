namespace SchoolPortal.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestLoggingMiddleware> logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

        await next(context);
    }
}
