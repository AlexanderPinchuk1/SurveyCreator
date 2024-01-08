using BSUIR.Survey.Domain.Pages;

namespace BSUIR.Survey.Domain.Surveys
{
    public class SurveyStatistics
    {
        public Guid SurveyId { get; set; }

        public string SurveyName { get; set; }

        public int AnswersCount { get; set; }

        public SurveyOptions Options { get; set; }

        public List<PageStatistics> PagesStatistics { get; set; }
    }
}
