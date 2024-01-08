namespace BSUIR.Survey.Domain.Users
{
    public class UserAnswersForSurvey
    {
        public List<UserAnswersPerPage> UserAnswersPerPages { get; set; }


        public UserAnswersForSurvey(List<UserAnswersPerPage> userAnswersPerPages)
        {
            UserAnswersPerPages = userAnswersPerPages;
        }
    }
}
