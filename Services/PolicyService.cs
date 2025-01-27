using AutoMapper;

using Contracts;
using Entities.Models;
using Entities.Exceptions;

using Shared.DTOs;
using Shared.RequestFeatures;

namespace Services;

/// <summary>
/// Handles the business logic for policies.
/// </summary>
public sealed class PolicyService : IPolicyService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public PolicyService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    
    /// <summary>
    /// Retrieves all policies through the repository.
    /// </summary>
    /// <returns>A list of policies.</returns>
    public async Task<IEnumerable<ReadPolicyDto>> GetAllPolicies(PolicyParameters parameters, bool trackChanges)
    {

        var policies = await _repository.Policy.GetAllPolicies(parameters,trackChanges);
        
        return _mapper.Map<IEnumerable<ReadPolicyDto>>(policies);

    }
    
    /// <summary>
    /// Gets a single policy and it's components from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the policy to retrieve.</param>
    /// <param name="trackChanges">Track the state of the retrieved entity or not.</param>
    /// <returns> A single policy and it's components </returns>
    public async Task<ReadPolicyDto> GetPolicy(int id, bool trackChanges)
    {
        var policy = await _repository.Policy.GetPolicy(id, trackChanges);
        if (policy is null)
        {
            throw new PolicyNotFoundException(id);
        }
        return _mapper.Map<ReadPolicyDto>(policy);
    }
    
    /// <summary>
    /// Adds a new policy through the repository.
    /// </summary>
    /// <param name="policyDto">The item to add.</param>
    /// <returns>The newly added policy with its generated ID.</returns>
    public async Task<ReadPolicyDto> CreatePolicy(CreatePolicyDto policyDto)
    {
        var getPolicy = await _repository.Policy.GetPolicyByName(policyDto.Name, false);
        if (getPolicy != null)
        {
            throw new PolicyAlreadyExistsException(policyDto.Name);
        }
        var policy = _mapper.Map<Policy>(policyDto);
        _repository.Policy.CreatePolicy(policy);
        await _repository.Save();
        
        return _mapper.Map<ReadPolicyDto>(policy);
    }

    /// <summary>
    /// Deletes a single policy and it's components from the database by ID.
    /// </summary>
    /// <param name="policyId">The ID of the policy to delete.</param>
    /// <param name="trackChanges">Track the state of the entity or not.</param>
    public async Task DeletePolicy(int policyId, bool trackChanges)
    {
        var policy = await  _repository.Policy.GetPolicy(policyId, trackChanges);
        if (policy is null)
        {
            throw new PolicyNotFoundException(policyId);
        }
        
        _repository.Policy.DeletePolicy(policy);
        await _repository.Save();
    }

    
    /// <summary>
    /// Updates a single policy and it's components from the database by ID.
    /// </summary>
    /// <param name="policyId">The ID of the policy to update.</param>
    /// <param name="trackChanges">Track the state of the retrieved entity or not.</param>
    /// <returns> The updated policy </returns>
    public async Task<ReadPolicyDto> UpdatePolicy(int policyId, CreatePolicyDto policyDto, bool trackChanges)
    {
        var policy = await _repository.Policy.GetPolicy(policyId, trackChanges);
        if (policy is null)
        {
            throw new PolicyNotFoundException(policyId);
        }

        int sequence = 1;
        if (policy.Components != null)
            foreach (var component in policy.Components)
            {
                component.Sequence = sequence;
                sequence++;
            }

        _mapper.Map(policyDto, policy);
        await _repository.Save();
        return _mapper.Map<ReadPolicyDto>(policy);
    }

    /// <summary>
    /// Get the benefits 
    /// </summary>
    /// <param name="requestQuote">The market value and policyID o.</param>
    /// <returns> Premium Benefits </returns>
    public async Task<ResponseQuote> CalculateBenefits(RequestQuoteDto requestQuote)
    {
        var policy = await _repository.Policy.GetPolicy(requestQuote.PolicyId, false);
        if (policy is null)
        {
            throw new PolicyNotFoundException(requestQuote.PolicyId);
        }
        
        decimal premium = 0.0m;

        if (policy.Components != null)
            foreach (var component in policy.Components.OrderBy(c => c.Sequence))
            {
                switch (component.Operation)
                {
                    case Operation.Add:
                        if (component.Name == ComponentName.MarketValuePremium)
                        {
                            premium += requestQuote.MarketValue * (component.PercentageValue / 100m);
                        }
                        else
                        {
                            premium += component.FlatValue;
                        }

                        break;

                    case Operation.Subtract:
                        if (component.Name == ComponentName.MarketValuePremium)
                        {
                            premium -= requestQuote.MarketValue * (component.PercentageValue / 100m);
                        }
                        else
                        {
                            premium -= component.FlatValue;
                        }

                        break;
                }
            }

        return new ResponseQuote(policy.Id, policy.Name, Math.Round(premium, 2));
        
    }
}