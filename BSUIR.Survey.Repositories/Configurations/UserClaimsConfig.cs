using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class UserClaimsConfig : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
        {
            builder.ToTable(name: "UserClaims");
        }
    }
}
