using System.ComponentModel.DataAnnotations;

namespace MeetMe.Models
{
    public class RegisterViewModel
    {
        [Required (ErrorMessage = "Email is required!")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(12)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        [Display(Name = "Password Confirmation")]
        public string PasswordConfirmation { get; set; }
    }
}
