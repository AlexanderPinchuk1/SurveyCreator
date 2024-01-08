using BSUIR.Repositories.UnitOfWork;
using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Pages;
using BSUIR.Survey.Domain.Questions;
using BSUIR.Survey.Domain.Surveys;
using BSUIR.Survey.Domain.Users;
using BSUIR.Survey.Repositories;
using Newtonsoft.Json;

namespace BSUIR.Survey.Foundation
{
    public class SurveyService : ISurveyService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISurveyUnitOfWork _surveyUnitOfWork;
        private readonly IUserService _userService;


        public SurveyService(ICurrentUserProvider currentUserProvider,
                                    IUnitOfWork unitOfWork,
                                    ISurveyUnitOfWork surveyUnitOfWork, 
                                    IUserService userService)
        {
            _currentUserProvider = currentUserProvider;
            _unitOfWork = unitOfWork;
            _surveyUnitOfWork = surveyUnitOfWork;
            _userService = userService;
        }


        public Domain.Surveys.Survey? FindSurveyOrReturnNull(Guid id)
        {
            var survey = _surveyUnitOfWork.GetSurveyRepository().GetSurveyIncludePagesAndQuestions(id);

            if (survey == null || (survey.Options & SurveyOptions.RandomOrderOfQuestions) == 0)
            {
                return survey;
            }

            foreach (var page in survey.Pages)
            {
                Shuffle(page.Questions);
                for (var i = 0; i < page.Questions.Count; i++)
                {
                    page.Questions[i].Number = i;
                }
            }

            return survey;
        }

        public SurveyTemplate? FindSurveyTemplateOrReturnNull(Guid id)
        {
            var survey = _surveyUnitOfWork.GetSurveyRepository().GetSurveyIncludePagesAndQuestions(id);

            if (survey == null)
            {
                return null;
            }

            var surveyTemplate = new SurveyTemplate()
            {
                Name = survey.Name,
                Options = survey.Options,
                PagesTemplates = survey.Pages.Select(page => new PageTemplate()
                {
                    Name = page.Name,
                    QuestionsTemplates = page.Questions
                        .Select(question => new QuestionTemplate()
                        {
                            Description = question.Description,
                            AvailableAnswers = question.AvailableAnswers,
                            IsRequired = question.IsRequired,
                            QuestionType = question.QuestionType,
                        }).ToList()
                }).ToList()
            };

            return surveyTemplate;
        }

        public async Task<List<AnswerError>?> SaveOrUpdateIfExistUserAnswers(Guid surveyId, List<UserAnswer> userAnswers)
        {
            var errors = GetErrorsIfNotAllRequiredQuestionsIsAnswered(surveyId, userAnswers);
            if (errors.Count != 0)
            {
                return errors;
            }

            CreateOrUpdateExistingAnswers(surveyId, userAnswers);

            await _unitOfWork.CommitAsync();

            return null;
        }

        private List<AnswerError> GetErrorsIfNotAllRequiredQuestionsIsAnswered(Guid surveyId, List<UserAnswer> userAnswers)
        {
            var requiredAnswers = _surveyUnitOfWork.GetSurveyRepository().GetSurveyIncludePagesAndQuestions(surveyId).Pages
                .SelectMany(page => page.Questions)
                .Where(question => question.IsRequired)
                .ToList();

            return (from requiredAnswer in requiredAnswers
                    where !userAnswers
                        .Exists(userAnswer => userAnswer.QuestionId == requiredAnswer.Id)
                    select new AnswerError() { QuestionId = requiredAnswer.Id, ErrorMessage = "Require answer!" })
                .ToList();
        }

        private void CreateOrUpdateExistingAnswers(Guid surveyId, IEnumerable<UserAnswer> userAnswers)
        {
            var userId = _currentUserProvider.GetUserId();

            var userAnswerRepository = _unitOfWork.GetRepository<UserAnswer>();

            var userAnswersId = Guid.NewGuid();

            if (userId != null)
            {
                var existAnswers = userAnswerRepository
                    .GetAll()
                    .Where(answer => answer.SurveyId == surveyId && answer.UserId == userId)
                    .ToList();

                userAnswers = SetUserAnswersId(userAnswers, existAnswers.Count == 0 ? userAnswersId : existAnswers.First().Id, userId);

                foreach (var userAnswer in userAnswers)
                {
                    var existAnswer = existAnswers.FirstOrDefault(answer => answer.QuestionId == userAnswer.QuestionId);

                    if (existAnswer != null)
                    {
                        existAnswer.Answer = userAnswer.Answer;
                    }
                    else
                    {
                        userAnswerRepository.Create(userAnswer);
                    }
                }

                var surveyResult = _unitOfWork.GetRepository<SurveyResult>()
                    .GetAll().FirstOrDefault(result => result.SurveyId == surveyId && result.UserId == userId);

                if (surveyResult != null)
                {
                    surveyResult.CompletionDate = DateTime.Now;
                }
                else
                {
                    _unitOfWork.GetRepository<SurveyResult>().Create(new SurveyResult()
                    {
                        SurveyId = surveyId,
                        UserId = (Guid)userId,
                        CompletionDate = DateTime.Now
                    });
                }
            }
            else
            {
                userAnswers = SetUserAnswersId(userAnswers, userAnswersId, null);

                foreach (var userAnswer in userAnswers)
                {
                    userAnswerRepository.Create(userAnswer);
                }
            }
        }

        private static IEnumerable<UserAnswer> SetUserAnswersId(IEnumerable<UserAnswer> userAnswers, Guid id, Guid? userId)
        {
            return userAnswers.Select(userAnswer => new UserAnswer()
            {
                Answer = userAnswer.Answer,
                Id = id,
                QuestionId = userAnswer.QuestionId,
                SurveyId = userAnswer.SurveyId,
                UserId = userId
            });
        }

        public List<UserAnswer> GetExistingUserAnswers(Guid surveyId)
        {
            var userId = _currentUserProvider.GetUserId();

            if (userId == null)
            {
                return new List<UserAnswer>();
            }

            var usersAnswersRepository = _unitOfWork.GetRepository<UserAnswer>();

            return usersAnswersRepository
                .GetAll()
                .Where(userAnswer => userAnswer.UserId == userId && userAnswer.SurveyId == surveyId)
                .ToList();
        }

        public async Task<UserAnswersForSurvey?> GetUserAnswersForSurvey(Guid surveyId, Guid userId)
        {
            if(surveyId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            var survey = _surveyUnitOfWork
               .GetSurveyRepository()
               .GetSurveyIncludePagesAndQuestions(surveyId);

            if (survey == null)
            {
                return null;
            }

            if(await _userService.FindUserByIdAsyncOrReturnNull(userId) == null)
            {
                return null;
            }

            var userAnswers = _unitOfWork.GetRepository<UserAnswer>()
                .GetAll()
                .Where(userAnswer => userAnswer.SurveyId == surveyId && userAnswer.UserId == userId)
                .ToList();

            return new UserAnswersForSurvey(survey.Pages
                .Select(page => new UserAnswersPerPage(page.Name, page.Questions
                    .Where(question => question != null)
                    .Select(question => new UserAnswersPerQuestion(question.Description, question.AvailableAnswers, question.QuestionType, question.IsRequired, userAnswers
                        .Where(answer => answer.QuestionId == question.Id)
                        .FirstOrDefault().Answer))
                    .ToList()))
                .ToList());
        }

        public SurveyStatistics? GetSurveyStatistics(Guid surveyId)
        {
            if(surveyId == Guid.Empty)
            {
                return null;
            }

            var survey = _surveyUnitOfWork
                .GetSurveyRepository()
                .GetSurveyIncludePagesAndQuestions(surveyId);

            if(survey == null)
            {
                return null;
            }

            var usersAnswers = _surveyUnitOfWork
                .GetUserAnswerRepository()
                .GetUserAnswerIncludeUserInfo(surveyId)
                .ToList();

            var surveyResultsCount = _unitOfWork
                .GetRepository<SurveyResult>()
                .GetAll()
                .Where(surveyResult => surveyResult.SurveyId == surveyId)
                .Count();

            var surveyStatistics = new SurveyStatistics()
            {
                SurveyId = surveyId,
                SurveyName = survey.Name,
                Options = survey.Options,
                AnswersCount = surveyResultsCount,
                PagesStatistics = survey.Pages.Select(page => new Domain.Pages.PageStatistics()
                {
                    PageName = page.Name,
                    PageId = page.Id,
                    Number = page.Number,
                    QuestionsStatistics = page.Questions.Select(question => new Domain.Questions.QuestionsStatistics()
                    {
                        QuestionId = question.Id,
                        Description = question.Description,
                        AnswersCount = usersAnswers.Where(surveyAnswer => surveyAnswer.QuestionId == question.Id).Count(),
                        IsRequired = question.IsRequired,
                        QuestionType = question.QuestionType
                    }).ToList()
                }).ToList()
            };

            return GenerateStatisticForQuestions(surveyStatistics, usersAnswers, survey.Pages.SelectMany(page => page.Questions).ToList());
        }

        private SurveyStatistics GenerateStatisticForQuestions(SurveyStatistics surveyStatistics, List<UserAnswer> usersAnswers, List<Domain.Questions.Question> questions)
        {
            foreach (var pageStatistics in surveyStatistics.PagesStatistics)
            {
                foreach (var questionStatistics in pageStatistics.QuestionsStatistics)
                {
                    questionStatistics.Keys = new List<string>();
                    questionStatistics.Values = new List<string>();
                    var usersAnswersToTheQuestion = usersAnswers.Where(question => question.QuestionId == questionStatistics.QuestionId).ToList();

                    switch (questionStatistics.QuestionType)
                    {
                        case QuestionType.OneAnswer:
                            {
                                var availableAnswers = questions.First(question => question.Id == questionStatistics.QuestionId).AvailableAnswers;
                                
                                if (availableAnswers == null)
                                {
                                    continue;
                                }

                                foreach (var availableAnswer in  availableAnswers)
                                {
                                    var percentOfTotalAnswersCount = usersAnswersToTheQuestion
                                        .Where(userAnswer => JsonConvert.DeserializeObject<string>(userAnswer.Answer) == availableAnswer)
                                        .Count() / (float) usersAnswersToTheQuestion.Count() * 100;

                                    questionStatistics.Keys.Add(Convert.ToString(percentOfTotalAnswersCount));
                                    questionStatistics.Values.Add(availableAnswer);
                                }
                            }
                            break;
                        case QuestionType.ManyAnswers:
                            {
                                var availableAnswers = questions.First(question => question.Id == questionStatistics.QuestionId).AvailableAnswers;

                                if (availableAnswers == null)
                                {
                                    continue;
                                }
                                var test = JsonConvert.DeserializeObject<List<string>>(usersAnswersToTheQuestion[0].Answer);

                                foreach (var availableAnswer in availableAnswers)
                                {
                                    questionStatistics.Keys.Add(Convert.ToString(usersAnswersToTheQuestion.Where(userAnswer =>
                                    {
                                        var answer = JsonConvert.DeserializeObject<List<string>>(userAnswer.Answer);

                                        return answer == null ? false : answer.Contains(availableAnswer);
                                    }).Count() / (float)usersAnswersToTheQuestion.Count() * 100));
                                    questionStatistics.Values.Add(availableAnswer);
                                }
                            }
                            break;
                        case QuestionType.Text:
                            {
                                foreach (var userAnswer in usersAnswersToTheQuestion)
                                {
                                    var userId = "";

                                    if(userAnswer.UserId != Guid.Empty)
                                    {
                                        userId = Convert.ToString(userAnswer.UserId);
                                    }

                                    var answer = JsonConvert.DeserializeObject<string>(userAnswer.Answer);
                                    if (answer == null)
                                    {
                                        continue;
                                    }
                                    
                                    questionStatistics.Keys.Add(answer);
                                    questionStatistics.Values.Add(userId == null ? "" : userId + " " + userAnswer.User.Email);
                                }
                            }
                            break;
                        case QuestionType.Rating:
                            {
                                var availableAnswers = new List<string>()
                                {
                                    "1",
                                    "2",
                                    "3",
                                    "4",
                                    "5"
                                };

                                foreach (var availableAnswer in availableAnswers)
                                {
                                    questionStatistics.Values.Add(Convert.ToString(Math.Round(usersAnswersToTheQuestion
                                        .Where(userAnswers => JsonConvert.DeserializeObject<string>(userAnswers.Answer) == availableAnswer)
                                        .Count() / (float)usersAnswersToTheQuestion.Count * 100)));
                                    questionStatistics.Keys.Add(Convert.ToString(availableAnswer) + " ☆");
                                }
                            }
                            break;
                        case QuestionType.Scale:
                            {
                                for(var i = 0; i < 10; i++)
                                {
                                    var firstBorder = 1 + i * 10;
                                    var secondBorder = (i + 1) * 10;

                                    questionStatistics.Keys.Add(Convert.ToString(firstBorder) + "-" + Convert.ToString(secondBorder));

                                    var answersCount = usersAnswersToTheQuestion.Where(userAnswer => Convert.ToInt32(JsonConvert.DeserializeObject<string>(userAnswer.Answer)) >= firstBorder
                                        && Convert.ToInt32(JsonConvert.DeserializeObject<string>(userAnswer.Answer)) <= secondBorder).Count();

                                    questionStatistics.Values.Add(Convert.ToString(answersCount / (float)usersAnswersToTheQuestion.Count * 100));
                                }
                            }
                            break;
                    }
                }
            }

            return surveyStatistics;
        }

        public bool UserIsAuthenticated()
        {
            return _currentUserProvider.IsAuthenticated();
        }

        public async Task AddSurvey(Domain.Surveys.Survey survey)
        {
            var userId = _currentUserProvider.GetUserId();
            if (userId == null || userId == Guid.Empty)
            {
                throw new InvalidOperationException("Error getting user id!");
            }

            survey.CreatedById = (Guid)userId;

            _unitOfWork.GetRepository<Domain.Surveys.Survey>().Create(survey);

            await _unitOfWork.CommitAsync();
        }

        public PagedEntities<SurveyInfo> GetSurveyInfoPerPageOrderedByDate(int pageIndex, int itemCountPerPage, string? searchKeyWord)
        {
            var userId = _currentUserProvider.GetUserId();
            if (userId == null || userId == Guid.Empty)
            {
                throw new InvalidOperationException("Error getting user id!");
            }

            List<Domain.Surveys.Survey> userSurveys  = _unitOfWork.GetRepository<Domain.Surveys.Survey>()
                .GetAll()
                .Where(survey => survey.CreatedById == userId && survey.IsTemplate == false)
                .OrderByDescending(survey => survey.CreationDateTime)
                .ToList();


            if (searchKeyWord != null)
            {
                userSurveys = userSurveys.Where(survey => survey.Name.Contains(searchKeyWord, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            itemCountPerPage = PaginationValidator.ValidateNumberOfItemsPerPage(itemCountPerPage);
            pageIndex = PaginationValidator.ValidateNumberOfPages(pageIndex, itemCountPerPage, userSurveys.Count);

            return new PagedEntities<SurveyInfo>(pageIndex, itemCountPerPage, userSurveys.Count, userSurveys.Skip(itemCountPerPage * pageIndex)
                .Take(itemCountPerPage).Select(survey => new SurveyInfo()
                {
                    Id = survey.Id,
                    CreationDateTime = survey.CreationDateTime.ToShortDateString(),
                    LinkToTheSurvey = "SurveyPassing?surveyId=" + survey.Id,
                    LinkToTheStatistics = "SurveyStatistics?surveyId=" + survey.Id,
                    Name = survey.Name,
                    AnswersCount = _unitOfWork.GetRepository<UserAnswer>().GetAll()
                        .Where(userAnswer => userAnswer.SurveyId == survey.Id)
                        .GroupBy(answer => answer.Id)
                        .Select(group => group.Key)
                        .Distinct().Count()
                }));
        }

        public List<User> GetUsersWhoPassedTheSurvey(Guid surveyId)
        {
            var usersId = _unitOfWork.GetRepository<UserAnswer>()
                .GetAll()
                .Where(userAnswer => userAnswer.SurveyId == surveyId)
                .Select(userAnswer => userAnswer.UserId)
                .Distinct()
                .ToList();

            return _unitOfWork.GetRepository<User>().GetAll().Where(user => usersId.Contains(user.Id)).ToList();
        }

        public async Task DeleteSurvey(Guid surveyId)
        {
            var userAnswers = _unitOfWork.GetRepository<UserAnswer>().GetAll()
                .Where(userAnswer => userAnswer.SurveyId == surveyId);

            foreach (var userAnswer in userAnswers)
            {
                _surveyUnitOfWork.GetUserAnswerRepository()
                    .DeleteUserAnswerUsingThreePartKey(userAnswer.Id, userAnswer.SurveyId, userAnswer.QuestionId);
            }

            _unitOfWork.GetRepository<Domain.Surveys.Survey>().Delete(surveyId);

            await _unitOfWork.CommitAsync();
        }


        private static void Shuffle<T>(IList<T> list)
        {
            var rand = new Random();

            for (var i = list.Count - 1; i >= 1; i--)
            {
                var j = rand.Next(i + 1);

                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public PagedEntities<SurveyTemplateInfo> GetSurveyTemplatesInfoPerPage(int pageIndex, int itemCountPerPage, string? searchKeyWord)
        {
            var userId = _currentUserProvider.GetUserId();
            if (userId == null || userId == Guid.Empty)
            {
                throw new InvalidOperationException("Error getting user id!");
            }

            var userSurveys = _surveyUnitOfWork.GetSurveyRepository().GetUserSurveyTemplatesIncludePagesAndQuestions((Guid)userId);

            if (searchKeyWord != null)
            {
                userSurveys = userSurveys.Where(survey => survey.Name.Contains(searchKeyWord, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            itemCountPerPage = PaginationValidator.ValidateNumberOfItemsPerPage(itemCountPerPage);
            pageIndex = PaginationValidator.ValidateNumberOfPages(pageIndex, itemCountPerPage, userSurveys.Count);

            return new PagedEntities<SurveyTemplateInfo>(pageIndex, itemCountPerPage, userSurveys.Count, userSurveys.Skip(itemCountPerPage * pageIndex)
                .Take(itemCountPerPage).Select(template => new SurveyTemplateInfo()
                {
                    TemplateName = template.Name,
                    QuestionCount = template.Pages.SelectMany(page => page.Questions).Count(),
                    PagesCount = template.Pages.Count(),
                    Id = template.Id
                }));
        }
    }
}