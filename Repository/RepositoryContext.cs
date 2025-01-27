using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Entities.Models;
using Repository.Configuration;

namespace Repository;


/// <summary>
/// The database context for the application, extending <see cref="IdentityDbContext{TUser}"/> 
/// to include Identity functionality and additional entity configurations.
/// </summary>
public class RepositoryContext: IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the model relationships, property conversions, and applies custom configurations.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure entity models.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PolicyComponent>().Property(c => c.Name).HasConversion<string>();
        modelBuilder.Entity<PolicyComponent>().Property(c => c.Operation).HasConversion<string>();
        
        modelBuilder.ApplyConfiguration(new PolicyConfiguration());
        modelBuilder.ApplyConfiguration(new ComponentConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());


    }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<PolicyComponent> Components { get; set; }
}