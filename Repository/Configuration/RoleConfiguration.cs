using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "11542211-d7a2-4ef2-92d7-5130804cac3b",
                Name = "Subscriber",
                NormalizedName = "SUBSCRIBER"
            },
            new IdentityRole
            {
                Id = "ddaaac62-e01b-4cea-bdd0-ddf314bc86ec",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );
    }
}