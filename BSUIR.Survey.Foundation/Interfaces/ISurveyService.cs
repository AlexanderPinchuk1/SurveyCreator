using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Surveys;
using BSUIR.Survey.Domain.Users;

namespace BSUIR.Survey.Foundation
{
    public interface ISurveyService
    {
        public Domain.Surveys.Survey? FindSurveyOrReturnNull(Guid id);

        public SurveyTemplate? FindSurveyTemplateOrReturnNull(Guid id);

        public Task<List<AnswerError>?> SaveOrUpdateIfExistUserAnswers(Guid surveyId, List<UserAnswer> userAnswers);

        public List<UserAnswer> GetExistingUserAnswers(Guid surveyId);

        public bool UserIsAuthenticated();

        public Task AddSurvey(Domain.Surveys.Survey survey);

        public PagedEntities<SurveyInfo> GetSurveyInfoPerPageOrderedByDate(int pageIndex, int itemCountPerPage, string? searchKeyWord);

        public PagedEntities<SurveyTemplateInfo> GetSurveyTemplatesInfoPerPage(int pageIndex, int itemCountPerPage, string? searchKeyWord);

        public Task DeleteSurvey(Guid surveyId);

        public SurveyStatistics? GetSurveyStatistics(Guid surveyId);

        public Task<UserAnswersForSurvey?> GetUserAnswersForSurvey(Guid surveyId, Guid userId);

        public List<User> GetUsersWhoPassedTheSurvey(Guid surveyId);
    }
}
