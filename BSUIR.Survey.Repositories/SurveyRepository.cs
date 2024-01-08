using BSUIR.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace BSUIR.Survey.Repositories
{
    public class SurveyRepository : Repository<Domain.Surveys.Survey>, ISurveyRepository
    {
        private readonly DbSet<Domain.Surveys.Survey> _dbSet;


        public SurveyRepository(SurveyDbContext surveyDbContext)
            : base(surveyDbContext)
        {
            _dbSet = surveyDbContext.Set<Domain.Surveys.Survey>();
        }


        public Domain.Surveys.Survey GetSurveyIncludePagesAndQuestions(Guid id)
        {
            return _dbSet.Include(survey => survey.Pages.OrderBy(page => page.Number))
                .ThenInclude(page => page.Questions.OrderBy(question => question.Number))
                .First(survey => survey.Id == id);
        }

        public List<Domain.Surveys.Survey> GetUserSurveyTemplatesIncludePagesAndQuestions(Guid userId)
        {
            return _dbSet.Include(survey => survey.Pages.OrderBy(page => page.Number))
                .ThenInclude(page => page.Questions.OrderBy(question => question.Number))
                .Where(survey => survey.IsTemplate == true && survey.CreatedById == userId)
                .ToList();
        }
    }
}
