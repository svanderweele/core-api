using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Core.Gaming.API.Controllers;

[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IGameCategoryRepository _repository;

    public CategoriesController(IGameCategoryRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet("")]
    public async Task<IEnumerable<GameCategory>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);
        return categories;
    }


    [HttpGet("{id:guid}", Name = "GetCategory")]
    public async Task<ActionResult<GameCategory>> Get(Guid id, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Create([FromBody] CreateGameCategoryRequest request, CancellationToken cancellationToken)
    {
        //TODO: Mapper for this
        var category = new GameCategory()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _repository.CreateAsync(category, cancellationToken);

        return CreatedAtRoute("GetCategory", routeValues: new { id = category.Id }, value: category);
    }

}