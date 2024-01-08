using BSUIR.Repositories.Repository;

namespace BSUIR.Survey.Repositories
{
    public interface ISurveyRepository : IRepository<Domain.Surveys.Survey>
    {
        public Domain.Surveys.Survey GetSurveyIncludePagesAndQuestions(Guid id);

        public List<Domain.Surveys.Survey> GetUserSurveyTemplatesIncludePagesAndQuestions(Guid userId);
    }
}
