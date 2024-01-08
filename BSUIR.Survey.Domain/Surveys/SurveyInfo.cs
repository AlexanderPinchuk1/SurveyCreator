namespace BSUIR.Survey.Domain.Surveys
{
    public class SurveyInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CreationDateTime { get; set; }

        public int AnswersCount { get; set; }

        public string LinkToTheSurvey { get; set; }

        public string LinkToTheStatistics { get; set; }
    }
}
