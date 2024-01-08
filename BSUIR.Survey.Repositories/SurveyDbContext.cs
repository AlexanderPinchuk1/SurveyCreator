using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Questions;
using BSUIR.Survey.Domain.Surveys;
using BSUIR.Survey.Repositories.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BSUIR.Survey.Repositories
{
    public class SurveyDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Domain.Surveys.Survey>? Surveys { get; set; }

        public DbSet<SurveyResult> SurveyResults { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Question> Questions { get; set; }

        private DbSet<QuestionTypeLookup> QuestionTypeLookups { get; set; }

        public DbSet<UserAnswer> UserAnswers { get; set; }


        public SurveyDbContext(DbContextOptions<SurveyDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                assembly: typeof(SurveyDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(
                assembly: typeof(Domain.Surveys.Survey).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(
               assembly: typeof(SurveyResult).Assembly);

            modelBuilder.Seed();
        }
    }
}
