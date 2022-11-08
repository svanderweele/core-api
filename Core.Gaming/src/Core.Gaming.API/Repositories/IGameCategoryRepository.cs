using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Repositories;

public interface IGameCategoryRepository
{
    
    Task<bool> CreateAsync(GameCategory category, CancellationToken cancellationToken);

    Task<IEnumerable<GameCategory>> GetAllAsync(CancellationToken cancellationToken);
    Task<GameCategory?> GetAsync(Guid id, CancellationToken cancellationToken);

    
}