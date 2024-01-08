using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class RoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
        {
            builder.ToTable(name: "RoleClaims");
        }
    }
}
