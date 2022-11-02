namespace Core.Authentication.API.Contracts.Responses;

public class ErrorResponse
{
    public string Message { get; }

    public string? Error { get;  }

    public string Code { get;  }

    public ErrorResponse(string code, string message, string? error = null)
    {
        Code = code;
        Message = message;
        Error = error;
    }
}