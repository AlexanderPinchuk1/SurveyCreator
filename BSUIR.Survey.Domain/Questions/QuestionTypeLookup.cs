using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSUIR.Survey.Domain.Questions
{
    public class QuestionTypeLookup
    {
        public QuestionType Id { get; set; }

        public string Name { get; set; }
    }

    public class QuestionTypeLookupConfig : IEntityTypeConfiguration<QuestionTypeLookup>
    {
        public void Configure(EntityTypeBuilder<QuestionTypeLookup> builder)
        {
            builder.ToTable("QuestionTypeLookup");
            builder.HasKey(questionType => questionType.Id);
            builder.Property(questionType => questionType.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(questionType => questionType.Name).IsUnique();
        }
    }
}
