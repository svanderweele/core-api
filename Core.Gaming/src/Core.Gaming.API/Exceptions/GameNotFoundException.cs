using System.Net;

namespace Core.Gaming.API.Exceptions;

public class GameNotFoundException : GamingException
{
    public GameNotFoundException(Guid gameId) : base("GAME_NOT_FOUND",
        $"Game with Id {gameId} not found.")
    {
        StatusCode = HttpStatusCode.NotFound;
        CustomData = new Dictionary<string, object>()
        {
            { "GameId", gameId },
        };
    }
}