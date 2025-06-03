using Microsoft.AspNetCore.Mvc;
using TimeManagement.Models;
using TimeManagement.Models.ViewModels;
using QRCoder;
using System.Text;
using TimeManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TimeManagement.Controllers
{
	public class AttendanceController : Controller
	{
		private readonly MyDbContext _context;

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
		public IActionResult Scan()
		{
			return View();
		}

		public IActionResult QRScanHandler()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				ViewBag.Message = "⚠️ You must be logged in to scan.";
				ViewBag.Status = "error";
				return View("QRResult");
			}

            var today = DateOnly.FromDateTime(DateTime.Today);
            var record = _context.Attendances.FirstOrDefault(a => a.UserId == userId && a.Date == today);

            // جلب IP الحالي
            var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            // التحقق إذا كان هناك موظف آخر استخدم نفس الـ IP اليوم
            //var conflict = _context.Users
            //    .Where(u => u.Ipaddress == currentIp && u.Id != userId)
            //    .Any();

            //if (conflict)
            //{
            //    ViewBag.Message = "⛔ This device has already been used for another employee today.";
            //    ViewBag.Status = "error";
            //    return View("QRResult");
            //}

			string message, status;

            if (record == null)
            {
                // تخزين IP المستخدم
                //var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                //if (user != null)
                //{
                //    user.Ipaddress = currentIp;
                //    user.UpdatedAt = DateTime.Now;
                //}

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

        public IActionResult QRCodes()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user == null) return NotFound();

			string qrText = $"{Request.Scheme}://{Request.Host}/Attendance/QRScanHandler";

			var generator = new QRCodeGenerator();
			var data = generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
			var svgQr = new SvgQRCode(data);
			string svgContent = svgQr.GetGraphic(4);
			string base64Svg = Convert.ToBase64String(Encoding.UTF8.GetBytes(svgContent));

			var viewModel = new UserQrViewModel
			{
				UserId = user.Id,
				FullName = user.FirstName + " " + user.LastName,
				QrBase64 = $"data:image/svg+xml;base64,{base64Svg}"
			};

			return View(viewModel);
		}

		// ✳️ إلغاء QRCheckIn(uid)
		public IActionResult QRCheckIn(int uid)
		{
			return RedirectToAction("QRScanHandler");
		}

		public IActionResult QRResult()
		{
			ViewBag.Status = "info";
			ViewBag.Message = "QR scan complete.";

			return View();
		}
	}

}
