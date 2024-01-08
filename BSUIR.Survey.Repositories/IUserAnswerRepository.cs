using BSUIR.Survey.Domain;

namespace BSUIR.Survey.Repositories
{
    public interface IUserAnswerRepository
    {
        public void DeleteUserAnswerUsingThreePartKey(Guid id, Guid surveyId, Guid questionId);

        public IQueryable<UserAnswer> GetUserAnswerIncludeUserInfo(Guid surveyId);
    }
}
