using Amazon.DynamoDBv2.Model;
using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Repositories;

public interface IGameRepository
{
    
    Task<bool> CreateAsync(Game game, CancellationToken cancellationToken);

    Task<ScanResponse?> GetAllAsync(CancellationToken cancellationToken,int limit, string? startKey);
    Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetByCollectionId(Guid collectionId, CancellationToken cancellationToken);
}