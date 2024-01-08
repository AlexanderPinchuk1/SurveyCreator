using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Domain.Surveys;
using BSUIR.Survey.Foundation;
using BSUIR.Survey.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSUIR.Survey.WebApp.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;


        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }


        public IActionResult SurveyPassing(Guid surveyId)
        {
            if (surveyId == Guid.Empty)
            {
                return BadRequest();
            }

            var survey = _surveyService.FindSurveyOrReturnNull(surveyId);

            if (survey == null)
            {
                return NotFound();
            }

            if ((survey.Options & SurveyOptions.Anonymity) == 0 && !_surveyService.UserIsAuthenticated())
            {
                return RedirectToAction("Login", "Account", new
                {
                    returnUrl = "/Survey/SurveyPassing?surveyId=" + surveyId
                });
            }

            var existingAnswers = _surveyService.GetExistingUserAnswers(surveyId);

            var model = new SurveyViewModel()
            {
                Id = survey.Id,
                Name = survey.Name,
                Options = survey.Options,
                Pages = survey.Pages.Select(page => new PageViewModel()
                {
                    Id = page.Id,
                    Name = page.Name,
                    Questions = page.Questions
                        .Select(question => new QuestionViewModel()
                        {
                            Question = question,
                            SurveyOptions = survey.Options,
                            Answer = existingAnswers.FirstOrDefault(existingAnswer => existingAnswer.QuestionId == question.Id)?.Answer
                        }).ToList()
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> SaveAnswers(Guid surveyId, List<UserAnswer> userAnswers)
        {
            if (surveyId == Guid.Empty || userAnswers.Any(userAnswer => userAnswer.SurveyId != surveyId))
            {
                return BadRequest();
            }

            var answersErrors = await _surveyService.SaveOrUpdateIfExistUserAnswers(surveyId, userAnswers);
            if (answersErrors == null)
            {
                return Ok();
            }

            return BadRequest(answersErrors);
        }

        [Authorize]
        public IActionResult SurveyCreation(Guid templateId)
        {
            if(templateId != Guid.Empty)
            {
                return View(_surveyService.FindSurveyTemplateOrReturnNull(templateId));
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> AddSurvey(Domain.Surveys.Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (survey.Pages
                .SelectMany(page => page.Questions)
                .Where(question => question.IsRequired == true)
                .Any() == false)
            {
                ModelState.AddModelError(string.Empty, "The survey does not contain required questions");

                return BadRequest(ModelState);
            }


            await _surveyService.AddSurvey(survey);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult SurveysList(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var model = _surveyService.GetSurveyInfoPerPageOrderedByDate(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);

            return View(model);
        }

        [HttpGet]
        public PagedEntities<SurveyInfo>? SurveysDataPerPage(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            var model = _surveyService.GetSurveyInfoPerPageOrderedByDate(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);

            return model;
        }

        [HttpGet]
        public PagedEntities<SurveyTemplateInfo>? SurveyTemplatesDataPerPage(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            var model = _surveyService.GetSurveyTemplatesInfoPerPage(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);

            return model;
        }


        [HttpGet]
        public IActionResult SurveyTemplatesList(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var model = _surveyService.GetSurveyTemplatesInfoPerPage(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);

            return View(model);
        }


        public async Task<IActionResult> DeleteSurvey(Guid surveyId, int itemCountPerPage, int pageIndex, string? searchKeyWord)
        {
            if (surveyId == Guid.Empty)
            {
                return BadRequest();
            }

            await _surveyService.DeleteSurvey(surveyId);

            return RedirectToAction("SurveysDataPerPage", new Pagination(itemCountPerPage, pageIndex, searchKeyWord));
        }

        public async Task<IActionResult> DeleteTemplate(Guid templateId, int itemCountPerPage, int pageIndex, string? searchKeyWord)
        {
            if (templateId == Guid.Empty)
            {
                return BadRequest();
            }

            await _surveyService.DeleteSurvey(templateId);

            return RedirectToAction("SurveyTemplatesDataPerPage", new Pagination(itemCountPerPage, pageIndex, searchKeyWord));
        }

        public IActionResult SurveyStatistics(Guid surveyId)
        {
            if (surveyId == Guid.Empty)
            {
                return BadRequest();
            }

            var survey = _surveyService.FindSurveyOrReturnNull(surveyId);
            if (survey == null)
            {
                return BadRequest();
            }

            return View(survey);
        }

        public SurveyStatistics? GetSurveyStatisticsData(Guid surveyId)
        {
            if (surveyId == Guid.Empty)
            {
                return null;
            }

            var surveyStatistics = _surveyService.GetSurveyStatistics(surveyId);

            if (surveyStatistics == null)
            {
                return null;
            }

            return surveyStatistics;
        }

        public List<User>? GetUsersWhoPassedTheSurvey(Guid surveyId)
        {
            if (surveyId == Guid.Empty)
            {
                return null;
            }

            return _surveyService.GetUsersWhoPassedTheSurvey(surveyId);
        }

        public async Task<Domain.Users.UserAnswersForSurvey?> GetUserAnswersForSurvey(Guid surveyId, Guid userId)
        {
            if (surveyId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            var userAnswersForSurvey = await _surveyService.GetUserAnswersForSurvey(surveyId, userId);

            if (userAnswersForSurvey == null)
            {
                return null;
            }

            return userAnswersForSurvey;
        }
    }
}
