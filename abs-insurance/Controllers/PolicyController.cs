using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Contracts;
using Shared.DTOs;
using Shared.RequestFeatures;
using Swashbuckle.AspNetCore.Annotations;

namespace abs_insurance.Controllers;

/// <summary>
/// Handles API endpoints related to policies.
/// </summary>
/// 
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
    
    
    /// <summary>
    /// Retrieves a paginated list of policies.
    /// </summary>
    /// <param name="requestParams">Query parameters for pagination.</param>
    /// <returns>A list of policies.</returns>
    /// <response code="200">Returns the list of policies.</response>
    [HttpGet("policy")]
    [SwaggerOperation(
        Summary = "Get Policies",
        Description = "Get a paginated list of policies."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully get a paginated list of policies.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [Authorize(Roles="Administrator,Subscriber")]
    public async Task<IActionResult> GetPolicies([FromQuery] PolicyParameters requestParams)
    {
        var companies = await _service.PolicyService.GetAllPolicies(requestParams,trackChanges: false);
        return Ok(companies);
    }
    
    /// <summary>
    /// Retrieves a specific policy by its ID.
    /// </summary>
    /// <param name="id">The ID of the policy.</param>
    /// <returns>The policy details.</returns>
    /// <response code="200">Returns the policy details.</response>
    /// <response code="404">Policy not found.</response>
    [HttpGet("policy/{id:int}", Name = "GetPolicy")]
    [SwaggerOperation(Summary = "Get Policy By Id", Description = "Get a policy by Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully returns a policy object.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Policy with provided ID not found.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [Authorize(Roles="Administrator,Subscriber")]
    public async Task<IActionResult> GetPolicy(int id)
    {
        var policy = await _service.PolicyService.GetPolicy(id, trackChanges: false);
        return Ok(policy);
    }
    
    
    /// <summary>
    /// Creates a new policy.
    /// </summary>
    /// <param name="policy">The policy to create.</param>
    /// <returns>The newly created policy.</returns>
    /// <response code="201">Policy successfully created.</response>
    /// <response code="400">Invalid request payload.</response>
    /// <response code="403">User is not authorized to create a policy.</response>
    [HttpPost("policy")]
    [SwaggerOperation(
        Summary = "Create New Policy",
        Description = "Create a new  policy. The policy details should be provided in the request body."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully created the policy.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data provided.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized to create policy.")]
    [Authorize(Roles="Administrator")]
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
    
    /// <summary>
    /// Updates an existing policy.
    /// </summary>
    /// <param name="policyId">The ID of the policy to update.</param>
    /// <param name="policy">The updated policy details.</param>
    /// <returns>The updated policy details.</returns>
    /// <response code="200">Policy successfully updated.</response>
    /// <response code="400">Invalid request payload.</response>
    /// <response code="403">User is not authorized to update the policy.</response>
    /// <response code="404">Policy not found.</response>
    [HttpPut("policy/{policyId:int}")]
    [SwaggerOperation(
        Summary = "Update Policy",
        Description = "Update a policy with provided request data."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully updated the policy.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data provided.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Policy with provided ID not found.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized to update policy.")]
    [Authorize(Roles="Administrator")]
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

    /// <summary>
    /// Deletes a policy by its ID.
    /// </summary>
    /// <param name="id">The ID of the policy to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Policy successfully deleted.</response>
    /// <response code="403">User is not authorized to delete the policy.</response>
    /// <response code="404">Policy not found.</response>
    [HttpDelete("policy/{id:int}")]
    [SwaggerOperation(
        Summary = "Delete Policy",
        Description = "Delete policy with provided policy ID."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Successfully deleted the policy.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Policy with provided ID not found.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized to create policy.")]
    [Authorize(Roles="Administrator")]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        await _service.PolicyService.DeletePolicy(id, trackChanges:false);
        return NoContent();
    }

    /// <summary>
    /// Requests a quote for a policy.
    /// </summary>
    /// <param name="quote">The quote request details.</param>
    /// <returns>A response with calculated benefits.</returns>
    /// <response code="200">Returns the calculated benefits.</response>
    /// <response code="400">Invalid request payload.</response>
    /// <response code="404">Policy not found.</response>
    [HttpPost("request-quote")]
    [SwaggerOperation(
        Summary = "Get Quote",
        Description = "Get calculated benefits wth provided ID and market value."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Successfully get quote.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Policy with provided ID not found.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated to make requests.")]
    [Authorize(Roles="Administrator,Subscriber")]
    public async Task<IActionResult> RequestQuote([FromBody] RequestQuoteDto quote)
    {
        ResponseQuote benefits = await  _service.PolicyService.CalculateBenefits(quote);
        return Ok(benefits);
    }
}