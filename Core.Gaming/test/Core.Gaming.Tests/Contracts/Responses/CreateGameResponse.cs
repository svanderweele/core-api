using System.Text.Json.Serialization;

namespace Core.Gaming.Tests.Contracts.Responses;

public class CreateGameResponse
{
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
        
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("display_index")]
    public int DisplayIndex { get; set; }
    
    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }
    
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
    
    //TODO: Consider splitting into its own db table or use enums
    [JsonPropertyName("devices")]
    public string[] Devices { get; set; }
    
    [JsonPropertyName("game_category")]
    public string GameCategory { get; set; }
    
    [JsonPropertyName("game_collections")]
    public IEnumerable<string> GameCollections { get; set; }



}