using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MeetMe.Data;
using MeetMe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MeetMe.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MeetingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager) //user manager przechowuje uzytkownikow, mozna nimi zarzadzac np nadawac role
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<IdentityUser> GetUser() => await _context.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

        // GET: Meetings
        public async Task<IActionResult> Index()
        {
            var user = await GetUser();
            var invitedMeetings = await _context.Attendance
                .Where(x => x.Attendee.Id == user.Id)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x.Meeting,
                    Confirmed = x.Confirmation,
                    IsAuthor = false
                })
                .ToListAsync();

            var userMeetings = await _context.Meeting
                .Where(x => x.Author.Id == user.Id)
                .Select(x => new MeetingViewModel
                {
                    Meeting = x,
                    IsAuthor = true,
                    Confirmed = true
                })
                .ToListAsync();
            
            return View(invitedMeetings.Concat(userMeetings));
        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.IsAuthor = false;
            var user = await GetUser();
            var meeting = await _context.Meeting
                .FirstOrDefaultAsync(m => m.Id == id && m.Author.Id == user.Id);

            if (meeting == null)
            {
                var attendance = await _context.Attendance.Include(x => x.Meeting)
                    .FirstOrDefaultAsync(x => x.Attendee.Id == user.Id && x.Meeting.Id == id);
                if(attendance == null)
                {
                    return NotFound();
                }
                meeting = attendance.Meeting;
            }
            else
            {
                ViewBag.IsAuthor = true;
            }

            ViewBag.Attendees = await _context.Attendance
                .Include(x => x.Attendee)
                .Where(x => x.Meeting.Id == id)
                .ToListAsync();

            return View(meeting);
        }

        // GET: Meetings/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> InviteUser(int? id)
        {
            if(id == null)
            {
                return BadRequest("Empty meet id");
            }

            var user = await GetUser();
            var meeting = await _context.Meeting.FirstOrDefaultAsync(x => x.Id == id && x.Author.Id == user.Id);
            if (meeting == null)
            {
                return NotFound("Nie znaleziono spotkania");
            }

            ViewBag.Users = new SelectList(await _userManager.Users.Where(x => x.Id != user.Id).ToListAsync(), "Id", "UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InviteUser(int? id, string guestId)
        {
            if (id == null)
            {
                return BadRequest("Empty meet id");
            }

            var user = await GetUser();
            var meeting = await _context.Meeting.FirstOrDefaultAsync(x => x.Id == id && x.Author.Id == user.Id);
            if(meeting == null)
            {
                return NotFound("Nie znaleziono spotkania");
            }

            var guest = await _userManager.FindByIdAsync(guestId);
            if(guest == null)
            {
                return NotFound("Nie znaleziono goscia");
            }

            ViewBag.Users = new SelectList(await _userManager.Users.Where(x => x.Id != user.Id).ToListAsync(), "Id", "UserName");

            var att = await _context.Attendance.FirstOrDefaultAsync(x => x.Attendee.Id == guest.Id && x.Meeting.Id == meeting.Id);
            if(att != null)
            {
                ViewBag.Message = "Nie możesz zaprosić tej osoby.";
                return View();
            }

            var attendance = new Attendance
            {
                Attendee = guest,
                Meeting = meeting,
            };
            await _context.AddAsync(attendance);
            await _context.SaveChangesAsync();

            ViewBag.MessageSuccess = "Dodano osobę.";
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,DateFrom,DateTo")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                //_userManager.GetUserId(User) - pobiera numer id w formie string uzytkownika ktory wykonal zapytanie na ten endpoint
                meeting.Author = await _context.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));
                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            var user = await GetUser();
            if(meeting.Author.Id != user.Id)
            {
                return Forbid();
            }

            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DateFrom,DateTo")] Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return NotFound();
            }

            var user = await GetUser();
            if (await _context.Meeting
                .Include(x => x.Author)
                .AnyAsync(x => x.Id == id && x.Author.Id == user.Id) == false)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);

            if (meeting == null)
            {
                return NotFound();
            }

            var user = await GetUser();
            if (meeting.Author.Id != user.Id)
            {
                return Forbid();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeetingConfirmation(int id)
        {
            var meeting = await _context.Meeting.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            var user = await GetUser();
            if (meeting.Author.Id != user.Id)
            {
                return Forbid();
            }

            var attendances = await _context.Attendance.Where(x => x.Meeting.Id == meeting.Id).ToListAsync();
            attendances.ForEach(x => _context.Remove(x));

            _context.Meeting.Remove(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Confirm(int? id)
        {
            if(id == null)
            {
                return BadRequest("Brak id");
            }

            var user = await GetUser();
            var attendance = await _context.Attendance.FirstOrDefaultAsync(x => x.Meeting.Id == id && x.Attendee.Id == user.Id);
            if (attendance == null)
            {
                return NotFound("Nie znaleziono spotkania.");
            }

            attendance.Confirmation = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAttendance(int? id) //id - id meetingu
        {
            if (id == null)
            {
                return BadRequest("Brak id");
            }

            var user = await GetUser();
            var attendance = await _context.Attendance
                .FirstOrDefaultAsync(x => x.Meeting.Id == id && x.Attendee.Id == user.Id);
            if (attendance == null)
            {
                return NotFound("Nie znaleziono spotkania.");
            }

            _context.Remove(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAttendanceByAuthor(int? id) //id - id uczestnictwa
        {
            if (id == null)
            {
                return BadRequest("Brak id");
            }

            var user = await GetUser();
            var attendance = await _context.Attendance
                .FirstOrDefaultAsync(x => x.Id == id && x.Meeting.Author.Id == user.Id);
            if (attendance == null)
            {
                return NotFound("Nie znaleziono spotkania.");
            }

            _context.Remove(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool MeetingExists(int id)
        {
            return _context.Meeting.Any(e => e.Id == id);
        }
    }
}
