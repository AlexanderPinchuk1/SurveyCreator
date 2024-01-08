using BSUIR.Survey.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Repositories.Configurations
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(user => user.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasIndex(user => user.Email).IsUnique();
            builder.ToTable(name: "Users");
        }
    }
}