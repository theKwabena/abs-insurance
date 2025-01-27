using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Contracts;
using Shared.DTOs;
using Shared.RequestFeatures;

namespace abs_insurance.Controllers;

[ApiController]
public class PolicyController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<CreatePolicyDto> _policyValidator;
    
    public PolicyController(IServiceManager service, IValidator<CreatePolicyDto> validator)
    {
        _service = service;
        _policyValidator = validator;
    }

    [HttpGet("policy")]
    public async Task<IActionResult> GetPolicies([FromQuery] PolicyParameters requestParams)
    {
        var companies = await _service.PolicyService.GetAllPolicies(requestParams,trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("policy/{id:int}", Name = "GetPolicy")]
    public async Task<IActionResult> GetPolicy(int id)
    {
        var policy = await _service.PolicyService.GetPolicy(id, trackChanges: false);
        return Ok(policy);
    }
    
    [HttpPost("policy")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyDto policy)
    {
        var validatePolicy = await _policyValidator.ValidateAsync(policy);
        if (!validatePolicy.IsValid)
        {
            return BadRequest(validatePolicy.ToDictionary());
        }
        var createdPolicy = await _service.PolicyService.CreatePolicy(policy);
        return CreatedAtRoute("GetPolicy", new { id = createdPolicy.Id }, createdPolicy);
    }
    
    [HttpPut("policy/{policyId:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdatePolicy(int policyId, [FromBody] CreatePolicyDto policy)
    {
        var validatePolicy = await _policyValidator.ValidateAsync(policy);
        if (!validatePolicy.IsValid)
        {
            return BadRequest(validatePolicy.ToDictionary());
        }
        var updatedPolicy = await _service.PolicyService.UpdatePolicy(policyId,policy,trackChanges: true);
        return CreatedAtRoute("GetPolicy", new { id = policyId }, updatedPolicy);
    }

    [HttpDelete("policy/{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        await _service.PolicyService.DeletePolicy(id, trackChanges:false);
        return NoContent();
    }

    [HttpPost("request-quote")]
    public async Task<IActionResult> RequestQuote([FromBody] RequestQuoteDto quote)
    {
        ResponseQuote benefits = await  _service.PolicyService.CalculateBenefits(quote);
        return Ok(benefits);
    }
}