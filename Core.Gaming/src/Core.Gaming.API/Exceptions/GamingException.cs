using System.Net;

namespace Core.Gaming.API.Exceptions;

public class GamingException : Exception
{
    public Dictionary<string, object>? CustomData { get; internal set; }

    public HttpStatusCode? StatusCode { get; internal set; }
    public string ErrorCode { get; internal set; }

    protected GamingException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}