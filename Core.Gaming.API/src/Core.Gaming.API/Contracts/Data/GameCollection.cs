using System.Text.Json.Serialization;

namespace Core.Gaming.API.Contracts.Data;

public class GameCollection
{

    [JsonPropertyName("id")] public Guid Id { get; set; }

    //If this collection is a sub-collection
    [JsonPropertyName("collection_id")] public Guid? CollectionId { get; set; }

    [JsonPropertyName("display_name")] public string DisplayName { get; set; }

    [JsonPropertyName("display_index")] public int DisplayIndex { get; set; }
}