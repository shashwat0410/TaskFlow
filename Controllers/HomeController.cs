using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Dashboard");
    }
}
