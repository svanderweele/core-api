using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Repositories;

namespace Core.Gaming.API.Services;

public class CollectionService : ICollectionService
{
    private readonly IGameCollectionRepository _repository;
    private readonly IGameService _gameService;

    public CollectionService(IGameCollectionRepository repository, IGameService gameService)
    {
        _repository = repository;
        _gameService = gameService;
    }

    //TODO: Try Nest Sub-Collections inside the model and retrieve by id 
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


    public async Task<GameCollectionDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await _repository.GetAsync(id, cancellationToken);

        if (collection == null) return null;

        return await FillCollection(collection, cancellationToken);
    }

    private async Task<GameCollectionDto> FillCollection(GameCollection collection,
        CancellationToken cancellationToken, List<Guid>? parentIds = null)
    {
        var games = await _gameService.GetByCollectionId(collection.Id, cancellationToken);
        var subCollections = await GetSubCollections(collection, cancellationToken, parentIds);
        return new GameCollectionDto(collection, subCollections, games);
    }

    public async Task<bool> CreateAsync(GameCollection collection, CancellationToken cancellationToken)
    {
        return await _repository.CreateAsync(collection, cancellationToken);
    }


    private async Task<IEnumerable<GameCollectionDto>> GetSubCollections(GameCollection parent,
        CancellationToken cancellationToken, List<Guid>? parentIds = null)
    {
        var collectionId = parent.Id;
        var subCollections = await _repository.GetSubCollections(collectionId, cancellationToken);

        parentIds ??= new List<Guid>();
        var convertedCollections = new List<GameCollectionDto>();

        //Don't load collection if it'll cause a recursive loop
        if (parentIds.Contains(collectionId)) return convertedCollections;

        parentIds.Add(collectionId);

        foreach (var subCollection in subCollections)
        {
            var collection = await FillCollection(subCollection, cancellationToken, parentIds);
            convertedCollections.Add(collection);
        }


        return convertedCollections;
    }
}