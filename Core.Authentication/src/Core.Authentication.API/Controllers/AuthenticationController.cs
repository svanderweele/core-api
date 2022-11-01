using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Authentication.API.Contracts.Data;
using Core.Authentication.API.Contracts.Requests;
using Core.Authentication.API.Contracts.Responses;
using Core.Authentication.API.Repositories;
using Core.Authentication.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Core.Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;
    private readonly IJwtService _jwtService;

    public AuthenticationController(ILogger<AuthenticationController> logger, ICustomerRepository customerRepository,
        IValidator<CreateCustomerRequest> createCustomerValidator, IJwtService jwtService)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _createCustomerValidator = createCustomerValidator;
        _jwtService = jwtService;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        await _createCustomerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingCustomer = await _customerRepository.GetAsync(request.Email, cancellationToken);

        if (existingCustomer != null)
        {
            //TODO: Custom exceptions
            throw new Exception("User with that email already exists");
        }

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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> GenerateTokenAsync(LoginRequest request,
        CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Debug, "Generate JWT Token!");

        var user = await _customerRepository.GetAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        //TODO: Check Password against Cognito?
        // if (user.Password != tokenRequest.Password) throw new Exception("Invalid Credentials!");

        var token = _jwtService.Generate(user);
        return new LoginResponse()
        {
            Token = token
        };
    }


    [Authorize]
    [HttpGet("", Name = "Get")]
    public async Task<ActionResult<CustomerDto>> Get(CancellationToken cancellationToken)
    {
        var id = User.Claims.SingleOrDefault(e => e.Type == ClaimTypes.Email);

        var customerId = id.Value;
        
        var customer = await _customerRepository.GetAsync(customerId, cancellationToken);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }
}