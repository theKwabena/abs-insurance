using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public enum Operation
{
    Add, Subtract
}

public enum ComponentName
{

    PremiumBase,
    ExtraPerils,
    MarketValuePremium,
    PromoDiscount
}
public class PolicyComponent
{
    public int Id { get; set; }
    
    public int Sequence { get; set; }

    public ComponentName Name { get; set; }
    public Operation Operation  { get; set; }
    
    public decimal FlatValue { get; set; }
    public decimal PercentageValue { get; set; }
    
    [ForeignKey(nameof(Policy))]
    public int PolicyId { get; set; }
    public Policy? Policy { get; set; }
}