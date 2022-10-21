namespace Core.Authentication.API.Contracts.Responses;

public class ErrorResponse
{
    private string Message { get; }

    private string? Error { get;  }

    public ErrorResponse(string message, string? error)
    {
        Message = message;
        Error = error;
    }
}