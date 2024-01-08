namespace BSUIR.Survey.WebApp.Models
{
    public class PageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
    }
}
