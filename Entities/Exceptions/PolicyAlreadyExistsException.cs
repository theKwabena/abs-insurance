namespace Entities.Exceptions;

public sealed class PolicyAlreadyExistsException : NotFoundException
{
    public PolicyAlreadyExistsException(string policyName) : base($"Policy with name {policyName} already exists.") {}
}