using FluentValidation;
using Microsoft.AspNetCore.Mvc;

using Contracts;
using Shared.DTOs;

namespace abs_insurance.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<UserRegistrationDto> _userRegistrationValidator;

    public AuthenticationController(IServiceManager service, IValidator<UserRegistrationDto> userRegistrationValidator)
    {
        _service = service;
        _userRegistrationValidator = userRegistrationValidator;
        
    } 

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userData)
    {
        var validateUser = await _userRegistrationValidator.ValidateAsync(userData);
        if (!validateUser.IsValid)
        {
            return BadRequest(validateUser.ToDictionary());
        }
        
        var result = await _service.AuthenticationService.RegisterUser(userData);
        if (!result.Succeeded)
        {
            
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
            
        }
        return StatusCode(201);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    {
        if (!await _service.AuthenticationService.ValidateUser(user))
            return Unauthorized();
        return Ok(new { Token = await _service
            .AuthenticationService.CreateToken() });
    }
}