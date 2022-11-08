using System.Text.Json.Serialization;

namespace Core.Gaming.Tests.Contracts.Requests;

public class CreateGameRequest
{
    [JsonPropertyName("name")] public string Name { get; init; } = default!;
    [JsonPropertyName("display_index")] public int DisplayIndex { get; init; } = default!;
    [JsonPropertyName("release_date")] public DateTime ReleaseDate { get; init; } = default!;
    [JsonPropertyName("game_category")] public Guid GameCategory { get; init; } = default!;

    [JsonPropertyName("thumbnail")] public string Thumbnail { get; init; } = default!;
    [JsonPropertyName("devices")] public string[] Devices { get; init; } = default!;
    [JsonPropertyName("collections")] public Guid[] Collections { get; init; } = default!;
}