using System.Text.Json.Serialization;

namespace Core.Gaming.API.Contracts.Responses;

public struct ErrorResponse
{
    [JsonPropertyName("code")] public string ErrorCode { get; set; }
    [JsonPropertyName("message")] public string Message { get; set;}
    [JsonPropertyName("stack")] public string? Error { get; set;}
    [JsonPropertyName("data")] public Dictionary<string, object>? Data { get; set;}
}