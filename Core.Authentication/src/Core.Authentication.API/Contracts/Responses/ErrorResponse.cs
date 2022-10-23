namespace Core.Authentication.API.Contracts.Responses;

public class ErrorResponse
{
    public string Message { get; }

    public string? Error { get;  }

    public ErrorResponse(string message, string? error)
    {
        Message = message;
        Error = error;
    }
}