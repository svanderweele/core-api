using System.Text.Json.Serialization;

namespace Core.Gaming.Tests.Contracts.Responses;

public class LoginResponse
{

    [JsonPropertyName("token")]
    public string Token { get; set; }
}