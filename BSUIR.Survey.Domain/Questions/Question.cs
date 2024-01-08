using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BSUIR.Survey.Domain.Questions
{
    public class Question
    {
        [GuidId]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The question must have a description.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The description of the question must contain from 1 to 100 characters.")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Number { get; set; }

        public bool IsRequired { get; set; }

        [AvailableAnswers]
        public List<string>? AvailableAnswers { get; set; }

        [Range(1, 6)]
        public QuestionType QuestionType { get; set; }

        public Page? Page { get; set; }
    }



    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Question");
            builder.HasKey(question => question.Id);
            builder.Property(question => question.Id).HasDefaultValueSql("newsequentialid()");
            builder.Property(question => question.Description).IsRequired();
            builder.Property(question => question.IsRequired).IsRequired();
            builder.Property(question => question.Number).IsRequired();
            builder.Property(question => question.QuestionType).IsRequired();
            builder.Property(question => question.AvailableAnswers)
                .HasConversion(availableAnswers => availableAnswers == null ? null : JsonConvert.SerializeObject(availableAnswers),
                    availableAnswers => availableAnswers == null ? null : JsonConvert.DeserializeObject<List<string>>(availableAnswers));
            builder.HasOne<QuestionTypeLookup>().WithMany().HasForeignKey(q => q.QuestionType);
            builder.HasOne(question => question.Page).WithMany(page => page.Questions).IsRequired();
        }
    }
}
