using Microsoft.AspNetCore.Mvc;

namespace BSUIR.Survey.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            return View("About");
        }
    }
}
