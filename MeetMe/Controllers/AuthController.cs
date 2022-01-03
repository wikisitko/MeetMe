using MeetMe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if(ModelState.IsValid) //Sprawdza czy model wprowadzony w argumencie jest poprawny wedlug adnotacji zawartej w klasie (RegisterViewModel)
            {
                var user = new IdentityUser { UserName = registerModel.Email, Email = registerModel.Email };
                var result = await userManager.CreateAsync(user, registerModel.Password);
                if(result.Succeeded)
                {
                    System.Console.WriteLine("Zarejestrowano");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description); //przesyla bledy z userManagera z powrotem do widoku formularza
                    System.Console.WriteLine(error.Description);
                }
            }

            return View(registerModel);
        }
    }
}
