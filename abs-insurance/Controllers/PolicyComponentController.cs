using Microsoft.AspNetCore.Mvc;
using FluentValidation;

using Contracts;
using Shared.DTOs;

namespace abs_insurance.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PolicyComponentController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<CreatePolicyComponentDto> _policyComponentValidator;
    
    public PolicyComponentController(IServiceManager service, IValidator<CreatePolicyComponentDto> policyComponentValidator)
    {
        _service = service;
        _policyComponentValidator = policyComponentValidator;
    }
    
    [HttpGet("{policyId:int}", Name = "GetComponentsByPolicy")]
    public async Task<IActionResult> GetComponentsByPolicy(int policyId)
    {
        var policies = await _service.PolicyComponentService.GetComponents(policyId, trackChanges: false);
        return Ok(policies);
    }
    
    // [HttpDelete("{policyId:int}/sequence/{sequence:int}")]
    // public IActionResult DeleteSequence(int policyId, int sequence)
    // {
    //     _service.PolicyComponentService.DeleteSequence(policyId,sequence, trackChanges:false);
    //     return NoContent();
    // }
    //
    // [HttpPost("{policyId:int}")]
    // public async Task<IActionResult> AddComponentToPolicy(int policyId, [FromBody] CreatePolicyComponentDto component)
    // {
    //     var validatePolicy = await _policyComponentValidator.ValidateAsync(component);
    //     if (!validatePolicy.IsValid)
    //     {
    //         return BadRequest(validatePolicy.ToDictionary());
    //     }
    //     var validatedComponent = _service.PolicyComponentService.AddPolicyToComponent(policyId,component, trackChanges:false);
    //     return CreatedAtRoute("GetPolicy", new { id = policyId }, validatedComponent);
    // }
}