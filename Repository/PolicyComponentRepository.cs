using Microsoft.EntityFrameworkCore;

using Contracts;
using Entities.Models;

namespace Repository;

public class PolicyComponentRepository : RepositoryBase<PolicyComponent>, IPolicyComponentRepository
{
    public PolicyComponentRepository(RepositoryContext repositoryContext):base(repositoryContext){}

    public async Task<IEnumerable<PolicyComponent>> GetPolicyComponents(int policyId, bool trackChanges) => await FindByCondition(
        e => e.PolicyId == policyId, trackChanges).OrderBy(e => e.Sequence).ToListAsync();

    public async Task<PolicyComponent> GetComponent(int policyId, int sequence, bool trackChanges) => await FindByCondition(
        e => e.PolicyId == policyId && e.Sequence == sequence, trackChanges).FirstOrDefaultAsync();
    
    public void DeletePolicyComponent(PolicyComponent component) => Delete(component);
    
    public void AddComponent(PolicyComponent component) => Create(component);
    
}