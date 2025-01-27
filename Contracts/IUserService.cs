using Microsoft.AspNetCore.Identity;

using Shared.DTOs;

namespace Contracts;

public interface IUserService
{
    Task<IdentityResult> CreateUser(UserCreationDto user);
}