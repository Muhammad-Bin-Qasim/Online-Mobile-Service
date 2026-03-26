using Microsoft.AspNetCore.Mvc;

namespace OnlineMobileServices.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Feedback() => View();
        public IActionResult Contact() => View();
        public IActionResult CustomerCare() => View();
        public IActionResult SiteMap() => View();
    }
}