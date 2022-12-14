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

    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger _logger;

    public GameService(IGameRepository repository, IGameCategoryRepository categoryRepository,
        IGameCollectionRepository collectionRepository, ILogger<GameService> logger, IConfiguration configuration)
    {
        
        _repository = repository;
        _categoryRepository = categoryRepository;
        _collectionRepository = collectionRepository;
        _logger = logger;
        
        var environment =  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Pipeline" == false)
        {
            var redisEndpoint = configuration.GetValue<string>("redis:endpoint");
            var multiplexer = ConnectionMultiplexer.Connect(redisEndpoint);
            _redis = multiplexer;
        }

    }

    //TODO: Try Nest Sub-Collections inside the model and retrieve by id 
    public async Task<GetAllGamesResponse> GetAllAsync(CancellationToken cancellationToken, int limit, string? startKey)
    {
        var scanResponse = await _repository.GetAllAsync(cancellationToken, limit, startKey);

        if (scanResponse == null)
        {
            throw new Exception("Internal Error with DynamoDb Query");
        }

        var dbGames = scanResponse.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<Game>(document.ToJson());
        }).OfType<Game>();

        var games = await Task.WhenAll(dbGames.Select(e => PopulateGame(e, cancellationToken)));

        LastEvaluatedKey? lastKey = null;

        if (scanResponse.LastEvaluatedKey.TryGetValue("id", out var value))
        {
            lastKey = new LastEvaluatedKey()
            {
                Key = "id",
                Value = value.S
            };
        }

        return new GetAllGamesResponse()
        {
            LastEvaluatedKey = lastKey,
            Games = games
        };
    }

    public async Task<GameSimpleDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await _repository.GetAsync(id, cancellationToken);

        if (game == null) return null;

        return await PopulateGame(game, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    private async Task<GameSimpleDto> PopulateGame(Game game,
        CancellationToken cancellationToken)
    {
        //TODO: Get multiple in one query
        var collections = new List<GameCollectionDto>();
        if (game.GameCollections != null)
        {
            foreach (var collectionId in game.GameCollections)
            {
                var collection = await _collectionRepository.GetAsync(collectionId, cancellationToken);
                if (collection != null)
                {
                    collections.Add(new GameCollectionDto(collection, Array.Empty<GameCollectionDto>(),
                        Array.Empty<GameSimpleDto>()));
                }
            }
        }

        var category = await _categoryRepository.GetAsync(game.GameCategory, cancellationToken);

        if (category == null)
        {
            //TODO: Custom Exceptions
            throw new Exception($"Game Category not found {game.Id}");
        }

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
        if (_redis == null)
        {
            throw new Exception("Redis is not initialised!");
        }

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
            throw new GameNotFoundException(gameId);
        }

        if (_redis == null)
        {
            throw new Exception("Redis is not initialised!");
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