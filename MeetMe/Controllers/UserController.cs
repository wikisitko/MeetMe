using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> AddRoleToUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var roles = roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            ViewBag.Email = user.Email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoleToUser(string email, string roleId)
        {
            if (email == null || roleId == null)
            {
                return Forbid();
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            await userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("Index");
        }
    }
}
