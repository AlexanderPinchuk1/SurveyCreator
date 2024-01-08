using BSUIR.Repositories.Repository;
using BSUIR.Survey.Domain;
using Microsoft.EntityFrameworkCore;

namespace BSUIR.Survey.Repositories
{
    public class UserAnswerRepository : Repository<UserAnswer>, IUserAnswerRepository
    {
        private readonly DbSet<UserAnswer> _dbSet;


        public UserAnswerRepository(SurveyDbContext surveyDbContext)
            : base(surveyDbContext)
        {
            _dbSet = surveyDbContext.Set<UserAnswer>();
        }


        public void DeleteUserAnswerUsingThreePartKey(Guid id, Guid surveyId, Guid questionId)
        {
            var item = _dbSet.Find(id, surveyId, questionId);
            if (item == null)
            {
                return;
            }

            _dbSet.Remove(item);
        }

        public IQueryable<UserAnswer> GetUserAnswerIncludeUserInfo(Guid surveyId)
        {
            return _dbSet.Include(item => item.User).Where(item => item.SurveyId == surveyId);
        }


    }
}
