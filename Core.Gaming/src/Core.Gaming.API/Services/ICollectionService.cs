using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Services;

public interface ICollectionService
{
    Task<IEnumerable<GameCollectionDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<GameCollectionDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CreateAsync(GameCollection coll, CancellationToken cancellationToken);
}