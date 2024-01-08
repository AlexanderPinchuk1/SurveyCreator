namespace BSUIR.Survey.Domain.Users
{
    public class UserAnswersPerPage
    {
        public string PageName { get; set; }

        public List<UserAnswersPerQuestion> UserAnswersPerQuestions{ get; set; }


        public UserAnswersPerPage(string pageName, List<UserAnswersPerQuestion> userAnswersPerQuestions)
        {
            PageName = pageName;
            UserAnswersPerQuestions = userAnswersPerQuestions;
        }
    }
}
