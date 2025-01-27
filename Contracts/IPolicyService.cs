using Shared.DTOs;
using Shared.RequestFeatures;

namespace Contracts;

public interface IPolicyService
{
    Task<IEnumerable<ReadPolicyDto>> GetAllPolicies(PolicyParameters parameters, bool trackChanges);
    Task<ReadPolicyDto> GetPolicy(int policyId, bool trackChanges);
    
    Task<ReadPolicyDto> CreatePolicy(CreatePolicyDto createPolicyDto);
    
    Task DeletePolicy(int policyId, bool trackChanges);

    Task<ReadPolicyDto> UpdatePolicy(int policyId, CreatePolicyDto policyDto, bool trackChanges);

    Task<ResponseQuote> CalculateBenefits(RequestQuoteDto requestQuote);
}