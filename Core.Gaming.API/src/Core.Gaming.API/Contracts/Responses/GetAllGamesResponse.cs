using System.Text.Json.Serialization;
using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Contracts.Responses;

[Serializable]
public struct LastEvaluatedKey
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class GetAllGamesResponse
{
    [JsonPropertyName(("games"))] public IEnumerable<GameSimpleDto> Games { get; set; }

    [JsonPropertyName(("last_evaluated_key"))]
    public LastEvaluatedKey? LastEvaluatedKey { get; set; }
}