using System.Text.Json.Serialization;

namespace Core.Authentication.API.Contracts.Data;

public class CustomerDto
{
    [JsonPropertyName("pk")]
    public string Pk => Id;

    [JsonPropertyName("sk")]
    public string Sk => Pk;

    [JsonPropertyName("id")]
    public string Id { get;  init;  } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;

    [JsonPropertyName("password")]
    public string Password { get; init; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; init; } = default!;

    [JsonPropertyName("date_of_birth")]
    public DateTime DateOfBirth { get; init; }
}