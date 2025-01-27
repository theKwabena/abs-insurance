using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IPolicyRepository> _policyRepository;
    private readonly Lazy<IPolicyComponentRepository> _policyComponentRepository;
    
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        
        _policyRepository = new Lazy<IPolicyRepository>(() => new
            PolicyRepository(repositoryContext));
        
        _policyComponentRepository = new Lazy<IPolicyComponentRepository>(() => new
            PolicyComponentRepository(repositoryContext));
    
    }
    public IPolicyRepository Policy => _policyRepository.Value;
    public IPolicyComponentRepository PolicyComponent => _policyComponentRepository.Value;
    
    public async Task Save() =>await  _repositoryContext.SaveChangesAsync();
}