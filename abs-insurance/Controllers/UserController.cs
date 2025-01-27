using Microsoft.AspNetCore.Mvc;
using FluentValidation;

using Contracts;
using Shared.DTOs;

namespace abs_insurance.Controllers;

public class UserController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<UserCreationDto> _userCreateDtoValidator;

    public UserController(IServiceManager service, IValidator<UserCreationDto> userRegistrationValidator)
    {
        _service = service;
        _userCreateDtoValidator = userRegistrationValidator;
    }
    
    [HttpPost("create-user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserCreationDto userData)
    {
        var validateUser = await _userCreateDtoValidator.ValidateAsync(userData);
        if (!validateUser.IsValid)
        {
            return BadRequest(validateUser.ToDictionary());
        }
        
        var result = await _service.UserService.CreateUser(userData);
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
}