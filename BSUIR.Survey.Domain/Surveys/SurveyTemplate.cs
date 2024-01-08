using BSUIR.Survey.Domain.Pages;

namespace BSUIR.Survey.Domain.Surveys
{
    public class SurveyTemplate
    {
        public string Name { get; set; }

        public SurveyOptions Options { get; set; }

        public List<PageTemplate> PagesTemplates { get; set; }
    }
}
