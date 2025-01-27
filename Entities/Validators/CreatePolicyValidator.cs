using System.ComponentModel;
using Entities.Models;
using Shared.DTOs;
using FluentValidation;

namespace Entities.Validators;

    public class CreatePolicyValidator : AbstractValidator<CreatePolicyDto>
    {
    public CreatePolicyValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Components)
            .NotNull()
            .WithMessage("At least one component is required")
            .Must(components =>
                components.Count == 4)
            .WithMessage("Each policy must have at exactly 4 components")
            .Must(components => components.All(c => IsValidComponent(c.Name)))
            .WithMessage("Policy must have 'Premium Base', 'ExtraPerils', 'Market Value Premium' and 'Promo Discount' components");
            
        RuleForEach(x => x.Components).SetValidator(new CreatePolicyComponentValidator());
    }

    private bool IsValidComponent(string componentName)
    {
        return Enum.TryParse<ComponentName>(componentName.Replace(" ", ""), true, out _);
    }
    
}