using BSUIR.Repositories.UnitOfWork;

namespace BSUIR.Survey.Repositories
{
    public class SurveyUnitOfWork : UnitOfWork, ISurveyUnitOfWork
    {
        private SurveyRepository? _surveyRepository;

        private UserAnswerRepository? _userAnswerRepository;

        private readonly SurveyDbContext _surveyDbContext;


        public SurveyUnitOfWork(SurveyDbContext dbContext)
            : base(dbContext)
        {
            _surveyDbContext = dbContext;
        }


        public ISurveyRepository GetSurveyRepository()
        {
            return _surveyRepository ??= new SurveyRepository(_surveyDbContext);
        }

        public IUserAnswerRepository GetUserAnswerRepository()
        {
            return _userAnswerRepository ??= new UserAnswerRepository(_surveyDbContext);
        }
    }
}