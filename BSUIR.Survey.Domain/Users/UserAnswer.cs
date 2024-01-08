using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Domain
{
    public class UserAnswer
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Question? Question { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public Guid SurveyId { get; set; }

        public Surveys.Survey? Survey { get; set; }

        public string Answer { get; set; }
    }



    public class UserAnswerConfig : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.ToTable("UsersAnswer");
            builder.HasOne(usersAnswer => usersAnswer.Survey).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(usersAnswer => usersAnswer.User).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(usersAnswer => usersAnswer.Question).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.Property<Guid>("SurveyId");
            builder.Property<Guid?>("UserId");
            builder.Property<Guid>("QuestionId");
            builder.HasKey(userAnswer => new { userAnswer.Id, userAnswer.SurveyId, userAnswer.QuestionId });
            builder.Property(userAnswer => userAnswer.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
