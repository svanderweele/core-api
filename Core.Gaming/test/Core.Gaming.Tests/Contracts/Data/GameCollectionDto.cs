using System.Text.Json.Serialization;
using Core.Gaming.Tests.Contracts.Data;

namespace Core.Gaming.API.Contracts.Data;

public class GameCollectionDto
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("display_name")] public string DisplayName { get; set; }

    [JsonPropertyName("display_index")] public int DisplayIndex { get; set; }

    [JsonPropertyName("sub_collections")] public IEnumerable<GameCollectionDto> SubCollections { get; set; }
    
}