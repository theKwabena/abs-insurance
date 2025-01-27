using FluentValidation;
using Microsoft.AspNetCore.Mvc;

using Contracts;
using Shared.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace abs_insurance.Controllers;

/// <summary>
/// Handles user authentication and registration.
/// </summary>
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IValidator<UserRegistrationDto> _userRegistrationValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
    /// </summary>
    /// <param name="service">Service manager for handling business logic.</param>
    /// <param name="userRegistrationValidator">Validator for user registration data.</param>
    public AuthenticationController(IServiceManager service, IValidator<UserRegistrationDto> userRegistrationValidator)
    {
        _service = service;
        _userRegistrationValidator = userRegistrationValidator;
        
    } 

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userData">The user registration data.</param>
    /// <returns>A status indicating the result of the registration.</returns>
    /// <response code="201">User successfully registered.</response>
    /// <response code="400">Invalid user data or registration failed.</response>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register New User",
        Description = "Register new user to make requests."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully created user.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data provided.")]
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
    
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="user">The user login data.</param>
    /// <returns>A JWT token if authentication is successful.</returns>
    /// <response code="200">Returns the generated JWT token.</response>
    /// <response code="401">Authentication failed. User credentials are invalid.</response>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Login - Get Access Token",
        Description = "Login user to get access token."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully logged in user.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid login credentials.")]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    {
        if (!await _service.AuthenticationService.ValidateUser(user))
            return Unauthorized();
        return Ok(new { Token = await _service
            .AuthenticationService.CreateToken() });
    }
}