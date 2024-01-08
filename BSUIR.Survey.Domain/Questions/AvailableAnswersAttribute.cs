using System.ComponentModel.DataAnnotations;

namespace BSUIR.Survey.Domain.Questions
{
    public class AvailableAnswersAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value,
            ValidationContext validationContext)
        {
            var question = ((Question)validationContext.ObjectInstance);
            var availableAnswers = (List<string>)value;

            if (availableAnswers == null)
            {
                if (question.QuestionType != QuestionType.OneAnswer && question.QuestionType != QuestionType.ManyAnswers)
                {
                    return ValidationResult.Success;
                }

                if (question.QuestionType == QuestionType.OneAnswer || question.QuestionType == QuestionType.ManyAnswers)
                {
                    return new ValidationResult("The question with description '" + question.Description + "' does not have available answers.");
                }
            }
            else
            {
                if (availableAnswers.Count != availableAnswers.Distinct().Count())
                {
                    return new ValidationResult("The question with description '" + question.Description + "' have repeated answers.");
                }
            }

            return availableAnswers.Any(availableAnswer => availableAnswer == null)
                ? new ValidationResult("The available answer to the question  with description '" + question.Description + "' must not be empty.") : ValidationResult.Success;
        }
    }
}
