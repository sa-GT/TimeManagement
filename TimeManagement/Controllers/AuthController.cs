using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using TimeManagement.Models;
using TimeManagement.ViewModels;
using System;
using Microsoft.EntityFrameworkCore;

namespace TimeManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly MyDbContext _context;

        public AuthController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            User? user = null;

            bool hasEmailAndPassword = !string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password);

            if (hasEmailAndPassword)
            {
                user = _context.Users.FirstOrDefault(u =>
                    u.Email.ToLower() == model.Email.ToLower() &&
                    u.Password == model.Password);
            }

            if (user == null)
            {
                ViewBag.Error = "❌ Invalid credentials";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("UserName", user.FirstName+" "+user.LastName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserImage", user.ProfilePicture ?? "/assets/images/profile_av.svg");


            return user.Role.ToLower() == "admin"
                 ? RedirectToAction("ViewAllEmployee", "HAdmin")
                : RedirectToAction("Profile", "Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


