using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Responses;

namespace Core.Gaming.API.Services;

public interface IGameService
{
    Task<GetAllGamesResponse> GetAllAsync(CancellationToken cancellationToken, int limit, string? startKey);

    Task<GameSimpleDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CreateAsync(Game game, CancellationToken cancellationToken);
    Task<IEnumerable<GameSimpleDto>> GetByCollectionId(Guid collectionId, CancellationToken cancellationToken);

    Task<PlayGameResponse?> ValidateGameSession(string sessionId);
    Task<PlayGameResponse> PlayGame(Guid userId, Guid gameId, CancellationToken cancellationToken);
}