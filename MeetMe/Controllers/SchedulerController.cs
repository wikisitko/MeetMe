using Microsoft.AspNetCore.Mvc;

namespace MeetMe.Controllers
{
    public class SchedulerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
