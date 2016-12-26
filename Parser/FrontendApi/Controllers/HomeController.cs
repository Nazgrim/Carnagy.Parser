using Microsoft.AspNetCore.Mvc;

namespace FrontendApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
