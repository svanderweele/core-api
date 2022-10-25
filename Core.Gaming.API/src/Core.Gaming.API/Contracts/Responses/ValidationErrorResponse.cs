namespace Core.Gaming.API.Contracts.Responses;

public class ValidationErrorResponse
{
    public List<string> Errors { get; init; } = new();
}