namespace Contracts;

public interface IServiceManager
{
    IPolicyService PolicyService { get; }
    IPolicyComponentService PolicyComponentService { get; }
    
    IAuthenticationService AuthenticationService { get; }
    IUserService UserService { get; }
}