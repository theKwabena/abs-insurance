using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Contracts;
using Shared.DTOs;
using Entities.Models;

namespace Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public UserService(IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        
    }
    
    public async Task<IdentityResult> CreateUser(UserCreationDto userData)
    {
        var newUser = _mapper.Map<User>(userData);
        
        var result = await _userManager.CreateAsync(newUser, userData.Password);
        var userRole = userData.IsAdmin ? "Administrator" : "Subscriber";
        if (result.Succeeded)
                await _userManager.AddToRoleAsync(newUser, userRole);
        return result;
    }
}