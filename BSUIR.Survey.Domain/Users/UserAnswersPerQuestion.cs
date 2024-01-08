using BSUIR.Survey.Domain.Questions;

namespace BSUIR.Survey.Domain.Users
{
    public class UserAnswersPerQuestion
    {
        public string Description{ get; set; }

        public List<string>? AvailableAnswers { get; set; }

        public QuestionType QuestionType { get; set; }

        public bool IsRequired { get; set; }

        public string? Answer { get; set; }


        public UserAnswersPerQuestion(string description, List<string>? availableAnswers,QuestionType questionType, bool isRequired, string? answer)
        {
            Description = description;
            AvailableAnswers = availableAnswers;
            Answer = answer;
            QuestionType = questionType;
            IsRequired = isRequired;
        }
    }
}
