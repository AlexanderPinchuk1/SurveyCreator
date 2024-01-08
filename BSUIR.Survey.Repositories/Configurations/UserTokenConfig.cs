using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class UserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
        {
            builder.ToTable(name: "UserTokens");
            builder.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });
        }
    }
}
