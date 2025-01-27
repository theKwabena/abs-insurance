namespace Entities.Exceptions;

public sealed class PolicyComponentNotFoundException : NotFoundException
{
    public PolicyComponentNotFoundException(int sequence) : base($"Sequence {sequence} not found.") {}

}