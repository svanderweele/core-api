using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Repositories;

namespace Core.Gaming.API.Services;

public class CollectionService : ICollectionService
{
    private readonly IGameCollectionRepository _repository;
    private readonly IGameRepository _gameRepository;

    public CollectionService(IGameCollectionRepository repository, IGameRepository gameRepository)
    {
        _repository = repository;
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<GameCollectionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var collections = await _repository.GetAllAsync(cancellationToken);
        var convertedCollections = new List<GameCollectionDto>();
        foreach (var gameCollection in collections)
        {
            var collectionDto = await GetAsync(gameCollection.Id, cancellationToken);
            if (collectionDto != null)
            {
                convertedCollections.Add(collectionDto);
            }
        }

        return convertedCollections;
    }


    public async Task<GameCollectionDto?> GetAsync(Guid id, CancellationToken cancellationToken,
        List<Guid>? parentIds = null)
    {
        var collection = await _repository.GetAsync(id, cancellationToken);

        if (collection == null) return null;

        var games = await _gameRepository.GetByCollectionId(collection.Id, cancellationToken);

        var subCollections = await GetSubCollections(collection.Id, cancellationToken, parentIds);
        return new GameCollectionDto(collection, subCollections, games);
    }

    public async Task<bool> CreateAsync(GameCollection collection, CancellationToken cancellationToken)
    {
        return await _repository.CreateAsync(collection, cancellationToken);
    }


    private async Task<IEnumerable<GameCollectionDto>> GetSubCollections(Guid parentId,
        CancellationToken cancellationToken, List<Guid>? parentIds = null)
    {
        var subCollections = await _repository.GetSubCollections(parentId, cancellationToken);

        parentIds ??= new List<Guid>();
        var convertedCollections = new List<GameCollectionDto>();
        
        //Don't load collection if it'll cause a recursive loop
        if (parentIds.Contains(parentId)) return convertedCollections;

        parentIds.Add(parentId);
        
        foreach (var subCollection in subCollections)
        {
            var collectionDto = await GetAsync(subCollection.Id, cancellationToken, parentIds);
            if (collectionDto != null)
            {
                convertedCollections.Add(collectionDto);
            }
        }


        return convertedCollections;
    }
}