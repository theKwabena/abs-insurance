using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Contracts;

namespace Repository;

/// <summary>
/// A base repository class that provides common CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The type of the entity that the repository will manage.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    /// <summary>
    /// The repository context used to interact with the database.
    /// </summary>
    protected RepositoryContext RepositoryContext;
    
    public RepositoryBase(RepositoryContext repositoryContext) => RepositoryContext = repositoryContext;
    
    
    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> from the database.
    /// </summary>
    /// <param name="trackChanges">Indicates whether to track changes to the entities.</param>
    /// <returns>A queryable collection of entities of type <typeparamref name="T"/>.</returns>
    public IQueryable<T> FindAll(bool trackChanges) => !trackChanges ?   
        RepositoryContext.Set<T>().AsNoTracking() 
        :
        RepositoryContext.Set<T>();
    
    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> that match the specified condition from the database.
    /// </summary>
    /// <param name="expression">A lambda expression to filter entities.</param>
    /// <param name="trackChanges">Indicates whether to track changes to the entities.</param>
    /// <returns>A queryable collection of entities that match the specified condition.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges) =>
        !trackChanges ?
            RepositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking() :
            RepositoryContext.Set<T>()
                .Where(expression); 
    
    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
    
    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
    
    /// <summary>
    /// Removes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);

}