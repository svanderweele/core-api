using Core.Gaming.API.Contracts.Requests;
using FluentValidation;

namespace Core.Gaming.API.Validation;

public class CreateGameRequestValidator: AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Collections).NotEmpty();
        RuleFor(x => x.Devices).NotEmpty();
        RuleFor(x => x.DisplayIndex).NotNull();
        RuleFor(x => x.GameCategory).NotEmpty();
        RuleFor(x => x.ReleaseDate).NotEmpty();
    }
}