using BSUIR.Repositories.UnitOfWork;
using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BSUIR.Survey.Foundation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISurveyUnitOfWork _surveyUnitOfWork;
        private readonly SurveyDbContext _surveyDbContext;

        public UserService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork,
            ISurveyUnitOfWork surveyUnitOfWork,
            SurveyDbContext surveyDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _surveyUnitOfWork = surveyUnitOfWork;
            _surveyDbContext = surveyDbContext;
        }


        public async Task<User> FindUserByIdAsyncOrReturnNull(Guid id)
        {
            return await _userManager.FindByIdAsync(Convert.ToString(id));
        }

        public async Task<IdentityResult> EditUserAsync(User user, string role)
        {
            if (await _roleManager.RoleExistsAsync(role) && !await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, role);
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            var userAnswers = _unitOfWork.GetRepository<UserAnswer>()
                .GetAll()
                .Where(userAnswer => userAnswer.UserId == user.Id);

            foreach (var userAnswer in userAnswers)
            {
                _surveyUnitOfWork
                    .GetUserAnswerRepository()
                    .DeleteUserAnswerUsingThreePartKey(userAnswer.Id, userAnswer.SurveyId, userAnswer.QuestionId);
            }

            await _unitOfWork.CommitAsync();

            return await _userManager.DeleteAsync(user);
        }

        public async Task<string> GetUserRoleAsync(User user)
        {
            return (await _userManager.GetRolesAsync(user)).First();
        }

        public async Task<bool> IsEmailUniqueAsync(Guid userId, string email)
        {
            return !await _userManager.Users.Where(user => user.Id != userId && user.Email == email).AnyAsync();
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddUserRole(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<PagedEntities<UserInfo>> GetUsersInfoForPageAsync(int pageIndex, int itemCountPerPage, string? searchKeyWord)
        {
            var users = new List<User>();

            if (searchKeyWord != null)
            {
                users = await _surveyDbContext.Users
                    .Where(user => user.Email.Contains(searchKeyWord))
                    .ToListAsync();
            }
            else
            {
                users = await _surveyDbContext.Users.ToListAsync();
            }

            var totalCount = users.Count;
            pageIndex = ValidateNumberOfPages(pageIndex, itemCountPerPage, totalCount);
            itemCountPerPage = ValidateNumberOfItemsPerPage(itemCountPerPage);

            var usersData = users
                .Skip(itemCountPerPage * pageIndex)
                .Take(itemCountPerPage)
                .Join(_surveyDbContext.UserRoles, user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new
                    {
                        user.Id,
                        user.RegistrationDateTime,
                        user.Email,
                        userRole.RoleId
                    })
                .Join(_surveyDbContext.Roles, user => user.RoleId, role => role.Id,
                    (user, role) => new
                    {
                        user.Id,
                        user.RegistrationDateTime,
                        user.Email,
                        Role = role.Name
                    }).ToList();

            return new PagedEntities<UserInfo>(pageIndex, itemCountPerPage, totalCount, usersData.Select(user => new UserInfo()
            {
                Id = user.Id,
                Role = user.Role,
                Email = user.Email,
                RegistrationDateTime = user.RegistrationDateTime.ToShortDateString(),
                CreatedSurveys = 0,
                CompletedSurveys = 0,
            }).ToList());
        }

        private static int ValidateNumberOfItemsPerPage(int numItemsPerPage)
        {
            numItemsPerPage = numItemsPerPage switch
            {
                < 1 => 1,
                > 100 => 100,
                _ => numItemsPerPage
            };

            return numItemsPerPage;
        }

        private static int ValidateNumberOfPages(int pageIndex, int itemCountPerPage, int totalCount)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            else if (pageIndex > Math.Ceiling((double)totalCount / itemCountPerPage) - 1)
            {
                pageIndex = (int)Math.Ceiling((double)totalCount / itemCountPerPage) - 1;
            }

            return pageIndex;
        }
    }
}

