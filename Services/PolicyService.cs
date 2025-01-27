using AutoMapper;

using Contracts;
using Entities.Models;
using Entities.Exceptions;

using Shared.DTOs;
using Shared.RequestFeatures;

namespace Services;

public sealed class PolicyService : IPolicyService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    


    public PolicyService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReadPolicyDto>> GetAllPolicies(PolicyParameters parameters, bool trackChanges)
    {

        var policies = await _repository.Policy.GetAllPolicies(parameters,trackChanges);
        
        return _mapper.Map<IEnumerable<ReadPolicyDto>>(policies);

    }

    public async Task<ReadPolicyDto> GetPolicy(int id, bool trackChanges)
    {
        var policy = await _repository.Policy.GetPolicy(id, trackChanges);
        if (policy is null)
        {
            throw new PolicyNotFoundException(id);
        }
        return _mapper.Map<ReadPolicyDto>(policy);
    }

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