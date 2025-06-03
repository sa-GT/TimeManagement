using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Text;
using TimeManagement.Models;
using TimeManagement.Models.ViewModels;
using TimeManagement.ViewModels;

namespace TimeManagement.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly MyDbContext _context;
        private static Dictionary<string, int> tokenStore = new(); // Token-to-UserId مؤقت

        public AttendanceController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Attendance()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var records = _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToList();

            var todayRecord = records.FirstOrDefault(a => a.Date == today);

            var model = new AttendanceViewModel
            {
                Records = records,
                TodayRecord = todayRecord,
                CanCheckIn = todayRecord == null,
                CanCheckOut = todayRecord != null && todayRecord.CheckIn.HasValue && !todayRecord.CheckOut.HasValue
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CheckIn()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (_context.Attendances.Any(a => a.UserId == userId && a.Date == today))
                return RedirectToAction("Attendance");

            _context.Attendances.Add(new Attendance
            {
                UserId = userId.Value,
                Date = today,
                CheckIn = DateTime.Now,
                Status = "present",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            _context.SaveChanges();
            return RedirectToAction("Attendance");
        }

        [HttpPost]
        public IActionResult CheckOut()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var record = _context.Attendances.FirstOrDefault(a => a.UserId == userId && a.Date == today);

            if (record == null || record.CheckOut.HasValue)
                return RedirectToAction("Attendance");

            record.CheckOut = DateTime.Now;
            record.UpdatedAt = DateTime.Now;

            if (record.CheckIn.HasValue)
            {
                var duration = (record.CheckOut.Value - record.CheckIn.Value).TotalHours;
                record.WorkHours = Math.Round((decimal)duration, 2);
            }

            _context.SaveChanges();
            return RedirectToAction("Attendance");
        }

        [HttpGet]
        public IActionResult Scan() => View();

        // ✅ QR Code Handler using Token
        public IActionResult QRScanHandler(string? token)
        {
            if (string.IsNullOrEmpty(token) || !tokenStore.ContainsKey(token))
            {
                ViewBag.Message = "❌ QR Code expired or invalid.";
                ViewBag.Status = "error";
                return View("QRResult");
            }

            int userId = tokenStore[token];
            tokenStore.Remove(token); // ♻️ احذف التوكن بعد الاستخدام

            var today = DateOnly.FromDateTime(DateTime.Today);
            var record = _context.Attendances.FirstOrDefault(a => a.UserId == userId && a.Date == today);

            string message, status;

            if (record == null)
            {
                _context.Attendances.Add(new Attendance
                {
                    UserId = userId,
                    Date = today,
                    CheckIn = DateTime.Now,
                    Status = "present",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                _context.SaveChanges();
                message = "✅ Check-in successful!";
                status = "success";
            }
            else if (record.CheckIn.HasValue && !record.CheckOut.HasValue)
            {
                record.CheckOut = DateTime.Now;
                record.UpdatedAt = DateTime.Now;

                var duration = (record.CheckOut.Value - record.CheckIn.Value).TotalHours;
                record.WorkHours = Math.Round((decimal)duration, 2);

                _context.SaveChanges();
                message = "✅ Check-out successful!";
                status = "success";
            }
            else
            {
                message = "ℹ️ You have already completed attendance today.";
                status = "info";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
            return View("QRResult");
        }

        // ✅ Dynamic QR Generator (used in <img src> via JS)
        [HttpGet]
        public IActionResult GetDynamicQr()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            string token = Guid.NewGuid().ToString();
            tokenStore[token] = userId.Value;

            string qrUrl = $"{Request.Scheme}://{Request.Host}/Attendance/QRScanHandler?token={token}";

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            var qrPng = new PngByteQRCode(data);
            byte[] qrBytes = qrPng.GetGraphic(20);

            return File(qrBytes, "image/png");
        }

        // ✅ صفحة عرض QR Code (تستخدم GetDynamicQr في <img>)
        public IActionResult QRCodes()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var viewModel = new UserQrViewModel
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                QrBase64 = "" // سيتم تحميله عبر <img src="/Attendance/GetDynamicQr">
            };

            return View(viewModel);
        }

        // ✅ إعادة التوجيه لو دخل شخص على رابط قديم
        public IActionResult QRCheckIn(int uid) => RedirectToAction("QRScanHandler");

        public IActionResult QRResult()
        {
            ViewBag.Status = "info";
            ViewBag.Message = "QR scan complete.";
            return View();
        }
    }
}
