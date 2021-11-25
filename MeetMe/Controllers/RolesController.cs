using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(IdentityRole identityRole)
        {
            if(string.IsNullOrWhiteSpace(identityRole.Name))
            {
                return BadRequest("Pusta nazwa");
            }

            var role = await roleManager.FindByNameAsync(identityRole.Name);
            if(role != null)
            {
                return BadRequest("Rola juz istnieje");
            }

            await roleManager.CreateAsync(identityRole);
            return Ok(identityRole.Id);
        }
    }
}
