using FluentValidation;
using Shared.DTOs;

namespace Entities.Validators;

public class UserCreationValidator  : AbstractValidator<UserCreationDto>
{
    public UserCreationValidator()
    {
        

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username cannot be empty")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

        RuleFor(x => x.CreateToken)
            .NotEmpty()
            .WithMessage("Token cannot be empty")
            .Must(BeCorrectCreationToken)
            .WithMessage("Token incorrect");
    }

    private bool BeCorrectCreationToken(string token)
    {
        var expectedToken = Environment.GetEnvironmentVariable("ADMIN_CREATION_TOKEN");
        return !string.IsNullOrEmpty(token) && token == expectedToken;
    }
}