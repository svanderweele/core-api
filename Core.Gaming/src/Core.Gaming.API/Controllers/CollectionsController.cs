using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.API.Repositories;
using Core.Gaming.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.Gaming.API.Controllers;

[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _service;

    public CollectionsController(ICollectionService service)
    {
        _service = service;
    }
    
    [HttpGet("")]
    public async Task<IEnumerable<GameCollectionDto>> GetAll(CancellationToken cancellationToken)
    {
        var collections = await _service.GetAllAsync(cancellationToken);
        return collections;
    }


    [HttpGet("{id:guid}", Name = "GetCollection")]
    public async Task<ActionResult<GameCollectionDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var collection = await _service.GetAsync(id, cancellationToken);

        if (collection == null)
        {
            return NotFound();
        }

        return collection;
    }

    // POST api/values
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameCollectionRequest request, CancellationToken cancellationToken)
    {
        //TODO: Mapper for this
        var collection = new GameCollection()
        {
            Id = Guid.NewGuid(),
            DisplayName = request.Name,
            DisplayIndex = request.DisplayIndex,
            CollectionId = request.SubCollection
        };

        await _service.CreateAsync(collection, cancellationToken);

        return CreatedAtRoute("GetCollection", routeValues: new { id = collection.Id }, value: collection);
    }

}