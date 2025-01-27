using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IPolicyRepository
{
    Task<IEnumerable<Policy>> GetAllPolicies(PolicyParameters parameters, bool trackChanges);
    Task<Policy> GetPolicy(int policyId, bool trackChanges);

    Task<Policy> GetPolicyByName(string policyName, bool trackChanges);
    
    void CreatePolicy(Policy policy);
    void DeletePolicy(Policy policyId);
}