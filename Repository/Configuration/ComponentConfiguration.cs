using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Entities.Models;

namespace Repository.Configuration;

public class ComponentConfiguration : IEntityTypeConfiguration<PolicyComponent>
{
    public void Configure(EntityTypeBuilder<PolicyComponent> builder)
    {
        builder.HasData(
            new PolicyComponent
            {
                Id = 1,
                PolicyId = 1,
                Operation = Operation.Add, 
                Name=ComponentName.ExtraPerils, 
                PercentageValue = 0,
                FlatValue = new decimal(300.00),
            }
            );
    }
    
}