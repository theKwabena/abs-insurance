namespace Shared.DTOs;

public record CreatePolicyComponentDto(string Name, string Operation, decimal FlatValue, decimal PercentageValue);