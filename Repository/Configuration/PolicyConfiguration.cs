using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Entities.Models;

namespace Repository.Configuration;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.HasData(
            new Policy { Id = 1, Name = "LowClaimPolicy" },
            new Policy { Id = 2, Name = "MediumClaimPolicy" },
            new Policy { Id = 3, Name = "HighClaimPolicy" }
        );
    }
    
}