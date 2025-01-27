using FluentValidation;
using Entities.Validators;

namespace abs_insurance.Extensions;

public static class ValidatorExtensions
{
    public static void ConfigureValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<CreatePolicyValidator>();
}