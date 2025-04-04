namespace Application.Core;

public class AppException
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; }

    public AppException(int statusCode, string message, string? stackTrace)
    {
        StatusCode = statusCode;
        Message = message;
        StackTrace = stackTrace;
    }
}