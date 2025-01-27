using Microsoft.EntityFrameworkCore;

using Contracts;
using Entities.Models;
using Shared.RequestFeatures;

namespace Repository;

/// <summary>
/// Provides access to policy data in the database.
/// </summary>
internal sealed class PolicyRepository : RepositoryBase<Policy>, IPolicyRepository
{
    public PolicyRepository(RepositoryContext repositoryContext) : base(repositoryContext){}
    
    /// <summary>
    /// Retrieves all policy from the database.
    /// </summary>
    public async Task<IEnumerable<Policy>> GetAllPolicies(PolicyParameters parameters,bool trackChanges) =>
        await FindAll(trackChanges)
            .OrderBy(p => p.Id)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(p => p.Components).ToListAsync();
    
    /// <summary>
    /// Gets a single policy and it's components from the database by ID.
    /// </summary>
    /// <param name="policyId">The ID of the policy to retrieve.</param>
    /// <param name="trackChanges">Track the state of the retrieved entity or not.</param>
    public async Task<Policy> GetPolicy(int policyId, bool trackChanges) => await FindByCondition(
        p => p.Id == policyId, trackChanges).Include(p => p.Components).SingleOrDefaultAsync();
    
    
    /// <summary>
    /// Gets a single policy and it's components from the database by name.
    /// </summary>
    /// <param name="policyName">The ID of the policy to retrieve.</param>
    /// <param name="trackChanges">Track the state of the retrieved entity or not.</param>
    public async Task<Policy> GetPolicyByName(string policyName, bool trackChanges) => await FindByCondition(
        p => p.Name == policyName, trackChanges).SingleOrDefaultAsync();

    
    /// <summary>
    /// Adds a new policy to the database.
    /// </summary>
    /// <param name="policy">The policy to add.</param>
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

    /// <summary>
    /// Deletes a policy to the database.
    /// </summary>
    /// <param name="policy">The policy to delete.</param>
    public void DeletePolicy(Policy policy) => Delete(policy);
}