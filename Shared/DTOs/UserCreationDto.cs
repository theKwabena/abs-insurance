namespace Shared.DTOs;

public record UserCreationDto
{
    public required string UserName { get; init; }
    public required string Email {get; init; }
    public required string Password { get; init; }
    public required bool IsAdmin { get; init; }
    public required string CreateToken { get; init; }
};