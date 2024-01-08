using BSUIR.Survey.Domain.Questions;

namespace BSUIR.Survey.Domain.Pages
{
    public class PageTemplate
    {
        public string Name { get; set; }

        public List<QuestionTemplate> QuestionsTemplates { get; set; }
    }
}
