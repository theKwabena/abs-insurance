namespace Shared.DTOs;

public record CreatePolicyDto(string Name, List<CreatePolicyComponentDto> Components);