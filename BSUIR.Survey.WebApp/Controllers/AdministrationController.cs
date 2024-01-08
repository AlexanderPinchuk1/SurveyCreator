using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using BSUIR.Survey.WebApp.Models;
using BSUIR.Survey.Foundation;

namespace BSUIR.Survey.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly IUserService _userService;


        public AdministrationController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await _userService.FindUserByIdAsyncOrReturnNull(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = await _userService.GetUserRoleAsync(user),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (model.Id == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await _userService.FindUserByIdAsyncOrReturnNull(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _userService.IsEmailUniqueAsync(model.Id, model.Email))
            {
                ModelState.AddModelError(string.Empty, "Email not unique!");

                return View(model);
            }

            user.Email = model.Email;

            var result = await _userService.EditUserAsync(user, model.Role);
            if (result.Succeeded)
            {
                return RedirectToAction("UsersList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await _userService.FindUserByIdAsyncOrReturnNull(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userService.DeleteUserAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("UsersDataPerPage");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("UsersDataPerPage");
        }

        [HttpGet]
        public async Task<IActionResult> UsersList(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var model = await _userService.GetUsersInfoForPageAsync(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);
            model.SearchKeyWord = pagination.SearchKeyWord;

            return View(model);
        }

        [HttpGet]
        public async Task<PagedEntities<UserInfo>?> UsersDataPerPage(Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            var model = await _userService.GetUsersInfoForPageAsync(pagination.PageIndex, pagination.ItemCountPerPage, pagination.SearchKeyWord);
            
            return model;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                RegistrationDateTime = DateTime.Now
            };

            var result = await _userService.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userService.AddUserRole(user, model.Role);

                return RedirectToAction("UsersList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public IActionResult CheckRole(string role)
        {
            return Json(role is Common.Role.User or Common.Role.Admin);
        }
    }
}