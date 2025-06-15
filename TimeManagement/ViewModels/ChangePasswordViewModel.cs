using System.ComponentModel.DataAnnotations;

namespace TimeManagement.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

}
