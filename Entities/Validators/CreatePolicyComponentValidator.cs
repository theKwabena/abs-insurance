using FluentValidation;

using Entities.Models;
using Shared.DTOs;

namespace Entities.Validators;

public class CreatePolicyComponentValidator : AbstractValidator<CreatePolicyComponentDto>
{
    public CreatePolicyComponentValidator()
    {
        RuleFor(x => x.FlatValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PercentageValue).InclusiveBetween(0, 100);
        RuleFor(x => x.Operation).Must(IsEnumValid).WithMessage(
            "Operation must be add or subtract.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }

    private bool IsEnumValid(string operation)
    {
        return Enum.TryParse<Operation>(operation, true, out _);
    }
}