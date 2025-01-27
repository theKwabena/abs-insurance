namespace Entities.Models;

public class Policy
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public IEnumerable<PolicyComponent>? Components { get; set; }
}