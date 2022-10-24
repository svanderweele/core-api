using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Repositories;
using Core.Gaming.API.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Gaming.API.Controllers;

[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _repository;
    private readonly IValidator<CreateGameRequest> _validator;

    public GamesController(IGameRepository repository, IValidator<CreateGameRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    
    [HttpGet("")]
    public async Task<IEnumerable<Game>> GetAll(CancellationToken cancellationToken)
    {
        var games = await _repository.GetAllAsync(cancellationToken);
        return games;
    }

    [HttpGet("{id:guid}", Name = "GetGame")]
    public async Task<ActionResult<Game>> Get(Guid id, CancellationToken cancellationToken)
    {
        var game = await _repository.GetAsync(id, cancellationToken);

        if (game == null)
        {
            //TODO: Custom exceptions
            throw new Exception($"Not Found with id {id}");
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

        await _repository.CreateAsync(game, cancellationToken);

        return CreatedAtRoute("GetGame", routeValues: new { id = game.Id }, value: game);
    }

    [Authorize]
    [HttpGet("play/{gameId}")]
    public async Task<Game?> PlayGame(Guid gameId, CancellationToken cancellationToken)
    {
        var game = await _repository.GetAsync(gameId, cancellationToken);
        return game;
    }
    
}