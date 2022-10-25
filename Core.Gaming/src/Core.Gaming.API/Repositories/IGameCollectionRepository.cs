using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Repositories;

public interface IGameCollectionRepository
{
    Task<bool> CreateAsync(GameCollection gameCollection, CancellationToken cancellationToken);

    Task<IEnumerable<GameCollection>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<GameCollection>> GetSubCollections(Guid parentId, CancellationToken cancellationToken);
    Task<GameCollection?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(GameCollection gameCollection, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}