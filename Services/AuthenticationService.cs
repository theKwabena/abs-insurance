using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using Contracts;
using Shared.DTOs;
using Entities.Models;

namespace Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    
    private User? _user;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<User> userManager, IConfiguration configuration, IMapper mapper)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<IdentityResult> RegisterUser(UserRegistrationDto userData)
    {
        var newUser = _mapper.Map<User>(userData);
        
        var result = await _userManager.CreateAsync(newUser, userData.Password);
        
        if (result.Succeeded)
            await _userManager.AddToRoleAsync(newUser, "Subscriber");
        
        return result;
    }
    
    public async Task<bool> ValidateUser(UserLoginDto loginCredentials)
    {
        _user = await _userManager.FindByNameAsync(loginCredentials.Username);
        var result = (_user != null && await _userManager.CheckPasswordAsync(_user,
            loginCredentials.Password));
        return result;
    }
    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ?? "ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM");
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
        List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }
}