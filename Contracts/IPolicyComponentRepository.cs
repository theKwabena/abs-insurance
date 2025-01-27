using Entities.Models;

namespace Contracts;

public interface IPolicyComponentRepository
{
    Task<IEnumerable<PolicyComponent>> GetPolicyComponents(int policyId, bool trackChanges);
    Task<PolicyComponent> GetComponent(int policyId, int sequence, bool trackChanges);
    
    void AddComponent(PolicyComponent component);

    void DeletePolicyComponent(PolicyComponent component);
}