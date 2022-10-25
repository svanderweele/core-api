namespace Core.Gaming.API.Contracts.Requests;

public class UpdateGameRequest
{
    public string Name { get; init; } = default!;
    public int DisplayIndex { get; init; } = default!;
    public DateTime ReleaseDate { get; init; } = default!;
    public Guid GameCategory { get; init; } = default!;

    public string Thumbnail { get; init; } = default!;
    public string[] ListOfDevices { get; init; } = default!;
    public Guid[] Collections { get; init; } = default!;
}