using Microsoft.AspNetCore.Mvc;
using FluentValidation;

using Contracts;
using Shared.DTOs;

namespace abs_insurance.Controllers;

public class UserController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<UserCreationDto> _userCreateDtoValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="service">Service manager for handling business logic.</param>
    /// <param name="userRegistrationValidator">Validator for user creation.</param>
    public UserController(IServiceManager service, IValidator<UserCreationDto> userRegistrationValidator)
    {
        _service = service;
        _userCreateDtoValidator = userRegistrationValidator;
    }
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userData">The user data for registration.</param>
    /// <returns>A status indicating the result of the operation.</returns>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Invalid user data or registration failed.</response>
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