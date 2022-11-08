namespace Core.Gaming.API.Contracts.Requests;

public class CreateGameCollectionRequest
{
    public string Name { get; init; } = default!;
    public int DisplayIndex { get; init; } = default!;
    public Guid? SubCollection { get; init; } = default!;
}