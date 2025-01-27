namespace Shared.DTOs;

public record ReadPolicyComponentDto(int Sequence, string Name, string Operation, decimal FlatValue, decimal PercentageValue);