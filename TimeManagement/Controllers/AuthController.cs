//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using System.Linq;
//using TimeManagement.Models;
//using TimeManagement.ViewModels;
//using System;

//namespace TimeManagement.Controllers
//{
//    public class AuthController : Controller
//    {
//        private readonly MyDbContext _context;

//        public AuthController(MyDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(LoginViewModel model)
//        {
//            User? user = null;

//            bool hasEmailAndPassword = !string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password);
//            bool hasFace = !string.IsNullOrWhiteSpace(model.FaceImageBase64);

//            // ✅ تسجيل الدخول بالبريد والباسورد فقط
//            if (hasEmailAndPassword && !hasFace)
//            {
//                user = _context.Users.FirstOrDefault(u =>
//                    u.Email.ToLower() == model.Email.ToLower() &&
//                    u.Password == model.Password);
//            }

//            // ✅ تسجيل الدخول بالوجه فقط
//            else if (hasFace && !hasEmailAndPassword)
//            {
//                var usersWithFace = _context.Users
//                    .Where(u => u.FaceImage != null)
//                    .ToList();

//                foreach (var u in usersWithFace)
//                {
//                    if (u.FaceImage == model.FaceImageBase64) // مقارنة نصية مبدئية
//                    {
//                        user = u;
//                        break;
//                    }
//                }
//            }

//            // ❌ لم يتم التحقق
//            if (user == null)
//            {
//                ViewBag.Error = "❌ Invalid credentials or face not recognized.";
//                return View(model);
//            }

//            // ✅ تسجيل دخول ناجح
//            HttpContext.Session.SetInt32("UserId", user.Id);
//            HttpContext.Session.SetString("Role", user.Role);

//            return user.Role == "admin"
//                ? RedirectToAction("EmployeeAttendanceList", "AdminAttendance")
//                : RedirectToAction("Profile", "Profile");
//        }

//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login");
//        }
//    }
//}
