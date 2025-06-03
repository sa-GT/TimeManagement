using Microsoft.AspNetCore.Mvc;
using TimeManagement.Models;
using Microsoft.AspNetCore.Http;
using TimeManagement.Models.ViewModels;

namespace TimeManagement.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MyDbContext _context;

        public ProfileController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Department = user.Department,
                Position = user.Position,
                LanguagePreference = user.LanguagePreference,
                CurrentImagePath = user.ProfilePicture
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Department = user.Department,
                Position = user.Position,
                LanguagePreference = user.LanguagePreference,
                CurrentImagePath = user.ProfilePicture
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.Department = model.Department;
            user.Position = model.Position;
            user.LanguagePreference = model.LanguagePreference;
            user.UpdatedAt = DateTime.Now;

            // ✅ حفظ صورة الوجه إذا كانت صحيحة
            if (!string.IsNullOrWhiteSpace(model.FaceImageBase64) && model.FaceImageBase64.StartsWith("data:image"))
            {
                user.FaceImage = model.FaceImageBase64;
            }

            // ✅ حفظ صورة الملف الشخصي
            if (model.ProfileImage != null)
            {
                var cleanFileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName)
                    .Replace(" ", "_").Replace("?", "").Replace(":", "").Replace("/", "").Replace("\\", "");

                var fileExt = Path.GetExtension(model.ProfileImage.FileName);
                var fileName = $"{Guid.NewGuid()}_{cleanFileName}{fileExt}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfilePicture = "/uploads/" + fileName;
            }

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}
