namespace Entities.Exceptions;

public sealed class PolicyNotFoundException : NotFoundException
{
    public PolicyNotFoundException(int policyId) : base($"Policy with id {policyId} not found.") {}
}