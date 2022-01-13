using MeetMe.Data;
using MeetMe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    [Authorize]
    public class SchedulerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SchedulerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        private async Task<IdentityUser> GetUser() => await _context.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

        private async Task<IEnumerable<MeetingViewModel>> GetMeetingsFromMonth(DateTime date)
        {
            var dateFrom = new DateTime(date.Year, date.Month, 1);
            var dateTo = dateFrom.AddMonths(1);

            var user = await GetUser();
            var invitedMeetings = await _context.Attendance
                .Where(x => x.Attendee.Id == user.Id && x.Meeting.DateFrom.Date >= dateFrom.Date && x.Meeting.DateFrom.Date < dateTo.Date)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x.Meeting,
                    Confirmed = x.Confirmation,
                    IsAuthor = false
                })
                .ToListAsync();

            var userMeetings = await _context.Meeting
                .Where(x => x.Author.Id == user.Id && x.DateFrom.Date >= dateFrom.Date && x.DateFrom.Date < dateTo.Date)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x,
                    IsAuthor = true,
                    Confirmed = true
                })
                .ToListAsync();

            return invitedMeetings.Concat(userMeetings);
        }

        private async Task<IEnumerable<MeetingViewModel>> GetMeetingsFromDay(DateTime date)
        {
            var user = await GetUser();
            var invitedMeetings = await _context.Attendance
                .Where(x => x.Attendee.Id == user.Id && x.Meeting.DateFrom.Date == date.Date)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x.Meeting,
                    Confirmed = x.Confirmation,
                    IsAuthor = false
                })
                .ToListAsync();

            var userMeetings = await _context.Meeting
                .Where(x => x.Author.Id == user.Id && x.DateFrom.Date == date.Date)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x,
                    IsAuthor = true,
                    Confirmed = true
                })
                .ToListAsync();

            return invitedMeetings.Concat(userMeetings);
        }

        public async Task<IActionResult> GetScheduler(int year, int month)
        {
            DateTime date = DateTime.Now;
            try
            {
                date = new DateTime(year, month, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var meetings = await GetMeetingsFromMonth(date);
            return PartialView("_schedulerPartial", new SchedulerViewModel { Date=date, Meetings=meetings});
        }

        public async Task<IActionResult> GetDayMeetings(int year, int month, int day)
        {
            DateTime date = DateTime.Now;
            try
            {
                date = new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var meetings = await GetMeetingsFromDay(date); //tytaj pobrac meetingi z dnia
            return PartialView("_moreMeetingsPartial", meetings); //tutaj przekazaac meeting z dnia
        }
    }
}
