namespace Shared.DTOs;

public record UserRegistrationDto
{
    public required string UserName { get; init; }
    
    public required string Email {get; init; }
    public required string Password { get; init; }
}