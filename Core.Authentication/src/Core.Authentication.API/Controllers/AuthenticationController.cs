using System.Security.Claims;
using Core.Authentication.API.Contracts.Data;
using Core.Authentication.API.Contracts.Requests;
using Core.Authentication.API.Contracts.Responses;
using Core.Authentication.API.Repositories;
using Core.Authentication.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;
    private readonly IJwtService _jwtService;

    public AuthenticationController(ICustomerRepository customerRepository,
        IValidator<CreateCustomerRequest> createCustomerValidator, IJwtService jwtService)
    {
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
            return Conflict(new ErrorResponse("AUTH_EMAIL_EXISTS", "User with that email already exists"));
        }

        //TODO: Use Automapper to map this
        var customer = new CustomerDto()
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Name = request.Name,
            Password = request.Password
        };

        await _customerRepository.CreateAsync(customer, cancellationToken);

        return CreatedAtRoute("Get", routeValues: new { id = customer.Id }, value: customer);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> GenerateTokenAsync(LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _customerRepository.GetAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return Unauthorized(new ErrorResponse("AUTH_INVALID_CREDENTIALS", "Invalid Credentials"));
        }

        //TODO: Check Password against Cognito?
        if (user.Password != request.Password)
        {
            return Unauthorized(new ErrorResponse("AUTH_INVALID_CREDENTIALS", "Invalid Credentials"));
        }

        var token = _jwtService.Generate(user);
        return new LoginResponse()
        {
            Token = token
        };
    }


    [Authorize]
    [HttpGet("me", Name = "Get")]
    public async Task<ActionResult<CustomerDto>> Get(CancellationToken cancellationToken)
    {
        var id = User.Claims.SingleOrDefault(e => e.Type == ClaimTypes.Email);

        var customerId = id.Value;

        var customer = await _customerRepository.GetAsync(customerId, cancellationToken);

        if (customer == null)
        {
            return NotFound(new ErrorResponse("AUTH_USER_NOT_FOUND", "User with that email was not found"));
        }

        return Ok(customer);
    }
}