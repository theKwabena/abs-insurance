using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Repository;

namespace abs_insurance.ContextFactory;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<RepositoryContext>().UseNpgsql(
            configuration.GetConnectionString("PostgresConnection"), b=> b.MigrationsAssembly("abs-insurance"));
        return new RepositoryContext(builder.Options);
    }
}