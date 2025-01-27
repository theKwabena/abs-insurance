using FluentValidation;
using Entities.Validators;

namespace abs_insurance.Extensions;

/// <summary>
/// Provides extension methods for configuring application validation.
/// </summary>
public static class ValidatorExtensions
{
    public static void ConfigureValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<CreatePolicyValidator>();
}