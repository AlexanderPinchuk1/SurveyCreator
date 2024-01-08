using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class UserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
        {
            builder.HasKey(key => new { key.ProviderKey, key.LoginProvider });
            builder.ToTable(name: "UserLogins");
        }
    }
}
