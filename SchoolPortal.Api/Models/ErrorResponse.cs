namespace SchoolPortal.Api.Models;

public class ErrorResponse
{
    public int ErrorCode { get; set; }
    public string? Message { get; set; }
    public string? InnerException { get; set; }
    public List<string>? Errors { get; set; }
}

