using System.Text.Json.Serialization;

namespace Core.Gaming.API.Contracts.Data;

public class GameCollectionDto
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("display_name")] public string? DisplayName { get; set; }

    [JsonPropertyName("display_index")] public int DisplayIndex { get; set; }

    [JsonPropertyName("sub_collections")] public IEnumerable<GameCollectionDto> SubCollections { get; set; }

    [JsonPropertyName("games")] public IEnumerable<GameSimpleDto> Games { get; set; }
    public GameCollectionDto(GameCollection collection, IEnumerable<GameCollectionDto> subCollections, IEnumerable<GameSimpleDto> games)
    {
        Id = collection.Id;
        DisplayName = collection.DisplayName;
        DisplayIndex = collection.DisplayIndex;
        SubCollections = subCollections;
        Games = games;
    }
}