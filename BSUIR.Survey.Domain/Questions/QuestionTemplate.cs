namespace BSUIR.Survey.Domain.Questions
{
    public class QuestionTemplate
    {
        public string? Description { get; set; }

        public bool IsRequired { get; set; }

        public List<string>? AvailableAnswers { get; set; }

        public QuestionType QuestionType { get; set; }
    }
}
