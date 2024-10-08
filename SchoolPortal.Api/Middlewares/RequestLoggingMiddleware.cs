namespace SchoolPortal.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Serilog.ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            logger.Information($"Request: {context.Request.Method} {context.Request.Path}");
            await next(context);
        }
    }
}
