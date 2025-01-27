using Microsoft.EntityFrameworkCore;

using Contracts;
using Entities.Models;
using Shared.RequestFeatures;

namespace Repository;

internal sealed class PolicyRepository : RepositoryBase<Policy>, IPolicyRepository
{
    public PolicyRepository(RepositoryContext repositoryContext) : base(repositoryContext){}
    
    public async Task<IEnumerable<Policy>> GetAllPolicies(PolicyParameters parameters,bool trackChanges) =>
        await FindAll(trackChanges)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(p => p.Components).ToListAsync();
    
    public async Task<Policy> GetPolicy(int policyId, bool trackChanges) => await FindByCondition(
        p => p.Id == policyId, trackChanges).Include(p => p.Components).SingleOrDefaultAsync();
    
    public async Task<Policy> GetPolicyByName(string policyName, bool trackChanges) => await FindByCondition(
        p => p.Name == policyName, trackChanges).SingleOrDefaultAsync();

    public void CreatePolicy(Policy policy)
    {
        int sequence = 1;
        if (policy.Components != null)
            foreach (var component in policy.Components)
            {
                component.Sequence = sequence;
                sequence++;
            }

        Create(policy);
    }

    public void DeletePolicy(Policy policy) => Delete(policy);
}