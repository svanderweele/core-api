using System.Net;
using Core.Gaming.API.Contracts.Data;

namespace Core.Gaming.API.Exceptions;

public class GameSessionAlreadyStartedException : GamingException
{
    public GameSessionAlreadyStartedException(Game game, string sessionId) : base("SESSION_ALREADY_STARTED",
        $"Player is already playing {game.DisplayName}")
    {
        StatusCode = HttpStatusCode.Conflict;
        CustomData = new Dictionary<string, object>()
        {
            { "Game", game },
            { "SessionId", sessionId }
        };
    }
}