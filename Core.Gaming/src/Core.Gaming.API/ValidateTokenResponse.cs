namespace Core.Gaming.API;

//TODO: use a shared library to share this response
public class ValidateTokenResponse
{
    public string Email { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Guid Id { get; init; } = default!;
}