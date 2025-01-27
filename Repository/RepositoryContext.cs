using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Entities.Models;
using Repository.Configuration;

namespace Repository;

public class RepositoryContext: IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

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