using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Responses;
using Core.Gaming.API.Exceptions;
using Core.Gaming.API.Repositories;
using StackExchange.Redis;

namespace Core.Gaming.API.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _repository;
    private readonly IGameCategoryRepository _categoryRepository;

    private readonly IGameCollectionRepository _collectionRepository;

    private IConnectionMultiplexer _redis;
    private readonly ILogger _logger;

    public GameService(IGameRepository repository, IGameCategoryRepository categoryRepository,
        IGameCollectionRepository collectionRepository, ILogger<GameService> logger, IConnectionMultiplexer redis)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _collectionRepository = collectionRepository;
        _redis = redis;
        _logger = logger;
    }

    //TODO: Try Nest Sub-Collections inside the model and retrieve by id 
    public async Task<GetAllGamesResponse> GetAllAsync(CancellationToken cancellationToken, string? startKey)
    {
        _logger.Log(LogLevel.Debug, "[B] Get All DB Games");

        var scanResponse = await _repository.GetAllAsync(cancellationToken, startKey);

        if (scanResponse == null)
        {
            throw new Exception("Internal Error with DynamoDb Query");
        }

        var dbGames = scanResponse.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<Game>(document.ToJson());
        }).OfType<Game>();

        _logger.Log(LogLevel.Debug, "[C] Got All DB Games");
        var games = await Task.WhenAll(dbGames.Select(e => PopulateGame(e, cancellationToken)));

        return new GetAllGamesResponse()
        {
            LastEvaluatedKey = scanResponse.LastEvaluatedKey != null
                ? new LastEvaluatedKey()
                {
                    Key = "id",
                    Value = scanResponse.LastEvaluatedKey["id"].S
                }
                : null,
            Games = games
        };
    }

    public async Task<GameSimpleDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await _repository.GetAsync(id, cancellationToken);

        if (game == null) return null;

        return await PopulateGame(game, cancellationToken);
    }

    private async Task<GameSimpleDto> PopulateGame(Game game,
        CancellationToken cancellationToken)
    {
        //TODO: Get multiple in one query
        var collections = new List<GameCollectionDto>();
        foreach (var collectionId in game.GameCollections)
        {
            _logger.Log(LogLevel.Debug, "[D] Get Collections");
            var collection = await _collectionRepository.GetAsync(collectionId, cancellationToken);
            if (collection != null)
            {
                collections.Add(new GameCollectionDto(collection, Array.Empty<GameCollectionDto>(),
                    Array.Empty<GameSimpleDto>()));
            }
        }

        _logger.Log(LogLevel.Debug, "[E] Got Collections");
        var category = await _categoryRepository.GetAsync(game.GameCategory, cancellationToken);

        if (category == null)
        {
            //TODO: Custom Exceptions
            throw new Exception($"Game Category not found {game.Id}");
        }

        _logger.Log(LogLevel.Debug, "[F] Got Game");
        return new GameSimpleDto(game, collections, category);
    }

    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken)
    {
        return await _repository.CreateAsync(game, cancellationToken);
    }

    public async Task<IEnumerable<GameSimpleDto>> GetByCollectionId(Guid collectionId,
        CancellationToken cancellationToken)
    {
        var games = await _repository.GetByCollectionId(collectionId, cancellationToken);
        var populatedGames = Task.WhenAll(games.Select(e => PopulateGame(e, cancellationToken)));
        return await populatedGames;
    }


    public async Task<PlayGameResponse?> ValidateGameSession(string sessionId)
    {
        var db = _redis.GetDatabase();
        var sessionRedisValue = await db.StringGetAsync($"GAME_PLAY_{sessionId}");

        if (sessionRedisValue == RedisValue.Null)
        {
            return null;
        }


        var sessionData = JsonSerializer.Deserialize<PlayGameResponse>(sessionRedisValue.ToString());

        if (sessionData == null)
        {
            return null;
        }

        sessionData.ExpiresIn = TimeSpan.FromSeconds(20);
        var stateData = JsonSerializer.Serialize(sessionData);
        await db.StringSetAsync($"GAME_PLAY_{sessionId}", stateData, sessionData.ExpiresIn);

        return sessionData;
    }

    public async Task<PlayGameResponse> PlayGame(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        var sessionId = $"{userId}-{gameId}";

        var game = await _repository.GetAsync(gameId, cancellationToken);

        if (game == null)
        {
            throw new Exception("Game Not Found!");
        }

        var db = _redis.GetDatabase();
        //Look for existing session
        var sessionRedisValue = await db.StringGetAsync($"GAME_PLAY_{sessionId}");

        if (sessionRedisValue != RedisValue.Null)
        {
            //TODO: Custom exception with game and sessionId
            throw new GameSessionAlreadyStartedException(game, sessionId);
        }


        var stringPool = new List<string>
        {
            "duck", "car", "chicken", "lazy", "pig", "hog", "dog", "angel", "core", "betsson", "tiger", "nugget",
            "golden", "miner", "deer", "hippo", "horse", "programmer",
        };

        string GenerateUrl(List<string> pool, int words)
        {
            if (pool.Count < words)
            {
                return string.Empty;
            }

            var word = "";
            var wordLength = 0;
            var random = new Random();
            while (wordLength < words)
            {
                var randomIndex = random.Next(0, pool.Count);

                if (wordLength > 0)
                {
                    word += "-";
                }

                word += pool[randomIndex];
                pool.RemoveAt(randomIndex);
                wordLength++;
            }

            return word;
        }

        var sessionData = new PlayGameResponse()
        {
            SessionId = sessionId,
            ExpiresIn = TimeSpan.FromSeconds(20),
            GameUrl = GenerateUrl(stringPool, 3)
        };

        var stateData = JsonSerializer.Serialize(sessionData);
        await db.StringSetAsync($"GAME_PLAY_{sessionId}", stateData, sessionData.ExpiresIn);

        return sessionData;
    }
}