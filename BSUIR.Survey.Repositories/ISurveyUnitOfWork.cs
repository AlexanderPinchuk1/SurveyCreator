namespace BSUIR.Survey.Repositories
{
    public interface ISurveyUnitOfWork
    {
        public ISurveyRepository GetSurveyRepository();

        public IUserAnswerRepository GetUserAnswerRepository();
    }
}
