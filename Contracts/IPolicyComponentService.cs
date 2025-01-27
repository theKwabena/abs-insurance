using Shared.DTOs;

namespace Contracts;

public interface IPolicyComponentService
{
    Task<IEnumerable<ReadPolicyComponentDto>> GetComponents(int policyId, bool trackChanges);

    // void DeleteSequence(int policyId, int componentSequence, bool trackChanges);
    //
    // Task<ReadPolicyDto> AddPolicyToComponent(int policyId, CreatePolicyComponentDto component, bool trackChanges);
}