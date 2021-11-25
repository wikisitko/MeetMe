using MeetMe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<IdentityUser> GetUser() => await _context.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            var attendance = await _context.Attendance
                .Include(x => x.Meeting)
                .Include(x => x.Attendee)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if(attendance == null)
            {
                return NotFound();
            }

            var user = await GetUser();
            if(attendance.Attendee.Id != user.Id && attendance.Meeting.Author.Id != user.Id)
            {
                return Forbid();
            }

            _context.Remove(attendance);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
