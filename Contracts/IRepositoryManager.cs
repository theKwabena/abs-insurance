namespace Contracts;

public interface IRepositoryManager
{
    IPolicyRepository Policy { get; }
    
    IPolicyComponentRepository PolicyComponent { get; }
    
    Task Save();
}