namespace BSUIR.Survey.Domain
{
    public class AnswerError
    {
        public Guid QuestionId { get; set; }

        public string ErrorMessage { get; set; }
    }
}
