namespace BSUIR.Survey.Domain.Questions
{
    public class QuestionsStatistics
    {
        public Guid QuestionId { get; set; }

        public string Description { get; set; }

        public int AnswersCount { get; set; }

        public bool IsRequired { get; set; }

        public QuestionType QuestionType { get; set; }

        public List<string> Keys{ get; set; }

        public List<string> Values { get; set; }

    }
}
