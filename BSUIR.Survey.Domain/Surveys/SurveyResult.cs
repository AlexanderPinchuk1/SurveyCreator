using BSUIR.Survey.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Domain.Surveys
{
    public class SurveyResult
    {
        [GuidId]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid SurveyId { get; set; }

        public Survey Survey { get; set; }

        public DateTime CompletionDate { get; set; }
    }



    public class SurveyResultConfig : IEntityTypeConfiguration<SurveyResult>
    {
        public void Configure(EntityTypeBuilder<SurveyResult> builder)
        {
            builder.ToTable("SurveyResult");
            builder.HasKey(surveyResult => surveyResult.Id);
            builder.Property(surveyResult => surveyResult.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasOne(surveyResult => surveyResult.Survey);
            builder.HasOne(surveyResult => surveyResult.User);
            builder.Property(surveyResult => surveyResult.CompletionDate).IsRequired();
            builder.Property<Guid>("UserId");
            builder.Property<Guid>("SurveyId");
            builder.HasKey("UserId", "SurveyId");
        }
    }
}
