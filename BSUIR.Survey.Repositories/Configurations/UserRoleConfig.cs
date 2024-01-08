using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class UserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.HasKey(key => new { key.UserId, key.RoleId });
            builder.ToTable(name: "UserRoles");
        }
    }
}
