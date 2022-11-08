using System.Text.Json.Serialization;

namespace Core.Gaming.API.Contracts.Responses;

public class PlayGameResponse
{
    [JsonPropertyName("session_id")] public string? SessionId { get; set; }
    [JsonPropertyName("game_url")] public string? GameUrl { get; set; }
    [JsonPropertyName("expiry")] public TimeSpan ExpiresIn { get; set; }
}