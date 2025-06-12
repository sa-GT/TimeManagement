namespace TimeManagement.Models.ViewModels
{
    public class ProfileViewModel
    {
        // معلومات البروفايل
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string Department { get; set; }
        public string Position { get; set; }
        public string LanguagePreference { get; set; }
        public string Username { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public string? CurrentImagePath { get; set; }
        public string? FaceImageBase64 { get; set; }

        // معلومات تغيير كلمة المرور
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
