using System.Security.Claims;
using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Contracts.Responses;
using Core.Gaming.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Gaming.API.Controllers;

[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _service;
    private readonly IValidator<CreateGameRequest> _validator;
    private readonly ILogger _logger;

    public GamesController(IGameService service, IValidator<CreateGameRequest> validator, ILogger<GamesController> logger)
    {
        _service = service;
        _validator = validator;
        _logger = logger;
    }

    
    [HttpGet("")]
    public async Task<GetAllGamesResponse> GetAll([FromQuery] string? startKey, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Debug, "[A] Get All Games");
        var games = await _service.GetAllAsync(cancellationToken, startKey);
        _logger.Log(LogLevel.Debug, "[H] Got All Games");
        return games;
    }

    [HttpGet("{id:guid}", Name = "GetGame")]
    public async Task<ActionResult<GameSimpleDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var game = await _service.GetAsync(id, cancellationToken);

        if (game == null)
        {
            return NotFound();
        }

        return game;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest request, CancellationToken cancellationToken)
    {
       await _validator.ValidateAndThrowAsync(request, cancellationToken);
        //TODO: Mapper for this
        //TODO: Upload Image as Base64 then add URL
        var game = new Game()
        {
            Id = Guid.NewGuid(),
            DisplayName = request.Name,
            DisplayIndex = request.DisplayIndex,
            Thumbnail = request.Thumbnail,
            GameCategory = request.GameCategory,
            Devices = request.Devices,
            GameCollections = request.Collections,
            ReleaseDate = request.ReleaseDate
        };

        await _service.CreateAsync(game, cancellationToken);

        return CreatedAtRoute("GetGame", routeValues: new { id = game.Id }, value: game);
    }

    [Authorize]
    [HttpGet("play/{gameId}")]
    public async Task<PlayGameResponse?> PlayGame(Guid gameId, CancellationToken cancellationToken)
    {
        if (gameId == Guid.Empty)
        {
            throw new Exception("Invalid Game Id");
        }
        
        var userId = Guid.Parse(this.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value);
        var game = await _service.PlayGame(userId, gameId, cancellationToken);
        return game;
    }
    
    [Authorize]
    [HttpGet("play/validate/{sessionId}")]
    public async Task<ActionResult<PlayGameResponse>> ValidateGameSession(string sessionId)
    {
        var session = await _service.ValidateGameSession(sessionId);

        if (session == null)
        {
            return NotFound();
        }
        return session;
    }
    
}