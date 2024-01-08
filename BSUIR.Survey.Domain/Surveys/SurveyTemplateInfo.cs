namespace BSUIR.Survey.Domain.Surveys
{
    public class SurveyTemplateInfo
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; }

        public int QuestionCount { get; set; }

        public int PagesCount { get; set; }
    }
}
