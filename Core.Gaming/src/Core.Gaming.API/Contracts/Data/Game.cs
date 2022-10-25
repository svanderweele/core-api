using System.Text.Json.Serialization;

namespace Core.Gaming.API.Contracts.Data;

public class Game
{
    public static string DEVICE_DESKTOP = "desktop";
    public static string DEVICE_MOBILE = "mobile";
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
        
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("display_index")]
    public int DisplayIndex { get; set; }
    
    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }
    
    [JsonPropertyName("game_category")]
    public Guid GameCategory { get; set; }
    
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
    
    //TODO: Consider splitting into its own table
    [JsonPropertyName("devices")]
    public string[] Devices { get; set; }

    [JsonPropertyName("collections")]
    public Guid[] GameCollections { get; set; }
}