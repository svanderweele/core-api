namespace Core.Authentication.API.Contracts.Responses;

public class ValidateTokenResponse
{
    public string Email { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Guid Id { get; init; } = default!;
}