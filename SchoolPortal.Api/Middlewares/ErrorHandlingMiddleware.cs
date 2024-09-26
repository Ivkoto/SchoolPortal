using Newtonsoft.Json;

namespace SchoolPortal.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //TODO @IvayloK Extend the status code logic. Add more than just 400.
            int statusCode = StatusCodes.Status400BadRequest;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                ErrorCode = statusCode,
                Message = exception.Message,
                InnerException = exception.InnerException?.Message,
                Errors = GetExceptionMessages(exception)
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
}
