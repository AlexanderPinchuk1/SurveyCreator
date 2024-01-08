using BSUIR.Survey.Domain.Questions;

namespace BSUIR.Survey.Domain.Pages
{
    public class PageStatistics
    {
        public Guid PageId { get; set; }

        public string PageName { get; set; }

        public int Number { get; set; }

        public List<QuestionsStatistics> QuestionsStatistics { get; set; } 
    }
}
