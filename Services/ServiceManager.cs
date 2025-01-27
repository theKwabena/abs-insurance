using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Contracts;
using Entities.Models;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IPolicyService> _policyService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    private readonly Lazy<IPolicyComponentService> _policyComponentService;
    private readonly Lazy<IUserService> _userService;

    public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper, UserManager<User> userManager,
        IConfiguration configuration)
    {
        _policyComponentService = new Lazy<IPolicyComponentService>(()=> 
            new PolicyComponentService(repositoryManager, mapper));
        
        _policyService = new Lazy<IPolicyService>(()=> 
            new PolicyService(repositoryManager, mapper));
        
        _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(userManager, configuration, mapper));
        
        _userService = new Lazy<IUserService>(()=> new UserService(mapper, userManager, configuration));
    }
    public IPolicyService PolicyService => _policyService.Value;
    
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IPolicyComponentService PolicyComponentService => _policyComponentService.Value;
    
    public IUserService UserService => _userService.Value;
    
}