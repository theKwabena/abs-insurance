using AutoMapper;

using Contracts;
using Shared.DTOs;
using Entities.Exceptions;

namespace Services;

internal sealed class PolicyComponentService : IPolicyComponentService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public PolicyComponentService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReadPolicyComponentDto>> GetComponents(int policyId, bool trackChanges)
    {
        var policy = await _repository.Policy.GetPolicy(policyId, trackChanges);
        if (policy == null)
        {
            throw new PolicyNotFoundException(policyId);
        }
        var employees = await _repository.PolicyComponent.GetPolicyComponents(
            policy.Id, trackChanges);
        return _mapper.Map<IEnumerable<ReadPolicyComponentDto>>(employees);
    }
    
    // public void DeleteSequence(int policyId, int componentSequence, bool trackChanges)
    // {
    //     var component = _repository.PolicyComponent.GetComponent(policyId, componentSequence, trackChanges);
    //     if (component == null)
    //     {
    //        throw new PolicyComponentNotFoundException(componentSequence);
    //     }
    //     _repository.PolicyComponent.DeletePolicyComponent(component);
    //     _repository.Save();
    // }
    //
    // public ReadPolicyDto AddPolicyToComponent(int policyId,  CreatePolicyComponentDto component, bool trackChanges)
    // {
    //     var policy = _repository.Policy.GetPolicy(policyId, trackChanges);
    //     if (policy == null)
    //     {
    //         throw new PolicyNotFoundException(policyId);
    //     }
    //
    //     int? sequence = policy.Components?.Count() + 1;
    //     var policyComponent = _mapper.Map<PolicyComponent>(component);
    //     policyComponent.PolicyId = policy.Id;
    //     policyComponent.Sequence = sequence ?? 1;
    //     _repository.PolicyComponent.AddComponent(policyComponent);
    //     _repository.Save();
    //     
    //     return _mapper.Map<ReadPolicyDto>(policy);
    // }
}