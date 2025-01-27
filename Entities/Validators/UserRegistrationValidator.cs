using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;

namespace Entities.Validators;

public class UserRegistrationValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationValidator()
    {

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username cannot be empty")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long");
    }
    
}