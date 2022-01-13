using System.ComponentModel.DataAnnotations;

namespace MeetMe.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required!")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
