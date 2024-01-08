using BSUIR.Survey.Domain.Surveys;

namespace BSUIR.Survey.WebApp.Models
{
    public class SurveyViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public SurveyOptions Options { get; set; }

        public List<PageViewModel> Pages { get; set; }
    }
}
