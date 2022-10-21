using Core.Authentication.API.Contracts.Data;
using Core.Authentication.API.Contracts.Requests;
using Core.Authentication.API.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;

    public AuthenticationController(ILogger<AuthenticationController> logger, ICustomerRepository customerRepository,
        IValidator<CreateCustomerRequest> createCustomerValidator)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _createCustomerValidator = createCustomerValidator;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        await _createCustomerValidator.ValidateAndThrowAsync(request, cancellationToken);
        
        //TODO: Use Automapper to map this
        var customer = new CustomerDto()
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth
        };

        _logger.Log(LogLevel.Information, "Got customer information {CustomerEmail}", customer.Email);

        await _customerRepository.CreateAsync(customer, cancellationToken);

        return CreatedAtRoute("Get", routeValues: new { id = customer.Id }, value: customer);
    }

    [HttpGet("", Name = "Get")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetAsync(id, cancellationToken);

        if (customer == null)
        {
            return NotFound();
        }
        
        return Ok(customer);
    }
}