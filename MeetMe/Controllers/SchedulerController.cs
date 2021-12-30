using Microsoft.AspNetCore.Mvc;
using System;

namespace MeetMe.Controllers
{
    public class SchedulerController : Controller
    {
        public IActionResult Index()
        {
            return View(DateTime.Now);
        }

        //[HttpGet("{id}")]
        //public IActionResult Index(string? id)
        //{
        //    return View(new DateTime(DateTime.Now.Year, int.Parse(id), 1));
        //}
    }
}
