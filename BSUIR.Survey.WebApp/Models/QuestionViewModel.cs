using BSUIR.Survey.Domain.Questions;
using BSUIR.Survey.Domain.Surveys;

namespace BSUIR.Survey.WebApp.Models
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }

        public SurveyOptions SurveyOptions { get; set; }

        public string Answer { get; set; }
    }
}
