using Microsoft.AspNetCore.Identity;

using Shared.DTOs;

namespace Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserRegistrationDto user);
    
    Task<bool> ValidateUser(UserLoginDto loginCredentials);
    Task<string> CreateToken();
}