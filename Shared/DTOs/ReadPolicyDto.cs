namespace Shared.DTOs;

public record ReadPolicyDto(int Id, string Name, IEnumerable<ReadPolicyComponentDto> Components);