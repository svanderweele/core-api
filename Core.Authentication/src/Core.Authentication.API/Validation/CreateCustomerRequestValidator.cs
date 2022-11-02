using Core.Authentication.API.Contracts.Requests;
using FluentValidation;

namespace Core.Authentication.API.Validation;

//TODO: Not currently being used but consider adding to controller for "" values
public class CreateCustomerRequestValidator: AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}