using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Questions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BSUIR.Survey.Repositories.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var adminRole = new Role()
            {
                Id = Guid.Parse("11ac23da-a8aa-47b4-a2a8-d32457760489"),
                Name = Common.Role.Admin,
                NormalizedName = Common.Role.Admin.ToUpper()
            };
            var userRole = new Role()
            {
                Id = Guid.Parse("aed7daac-9ce0-496f-a606-7b79d37dcbc1"),
                Name = Common.Role.User,
                NormalizedName = Common.Role.User.ToUpper()
            };
            modelBuilder.Entity<Role>().HasData(adminRole, userRole);

            var admin = new User
            {
                Id = Guid.Parse("1f363ed7-59b2-460c-91a6-fcd30a2c3872"),
                UserName = "Admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "Admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                RegistrationDateTime = DateTime.Now,
                SecurityStamp = "7d90869b-19c6-4cb0-8c74-790e8352fabe"
            };
            var user = new User()
            {
                Id = Guid.Parse("7ba77241-b5d6-4490-aa85-0493c6acdbf3"),
                UserName = "User@gmail.com",
                NormalizedUserName = "USER@GMAIL.COM",
                Email = "User@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                RegistrationDateTime = DateTime.Now,
                SecurityStamp = "4010b6f9-59e8-42b9-bf76-97c41907189b"
            };
            var passwordHasher = new PasswordHasher<User>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin1234!");
            user.PasswordHash = passwordHasher.HashPassword(user, "User1234!");
            modelBuilder.Entity<User>().HasData(admin, user);

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>() { RoleId = userRole.Id, UserId = user.Id },
                new IdentityUserRole<Guid>() { RoleId = adminRole.Id, UserId = admin.Id }
            );

            modelBuilder.Entity<QuestionTypeLookup>().HasData(new List<QuestionTypeLookup>()
            {
                new()
                {
                    Id = QuestionType.Text,
                    Name = "Text"
                },
                new()
                {
                    Id = QuestionType.OneAnswer,
                    Name = "OneAnswer"
                },
                new()
                {
                    Id = QuestionType.ManyAnswers,
                    Name = "ManyAnswers"
                },
                new()
                {
                    Id = QuestionType.Scale,
                    Name = "Scale"
                },
                new()
                {
                    Id = QuestionType.Rating,
                    Name = "Rating"
                }
            });
        }
    }
}
