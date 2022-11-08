using System.Text.Json.Serialization;

namespace Core.Gaming.Tests.Contracts.Data;

public class GameCategory
{
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}