using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using System.Text;
using TimeManagement.Models;
using TimeManagement.ViewModels;

namespace TimeManagement.Controllers
{
    public class AdminAttendanceController : Controller
    {
        private readonly MyDbContext _context;

        public AdminAttendanceController(MyDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult EmployeeAttendanceList()
        {
            var attendanceData = _context.Attendances
                .Include(a => a.User)
                .OrderByDescending(a => a.Date)
                .Select(a => new AdminAttendanceViewModel
                {
                    UserId = a.Id,
                    FullName = a.User.FirstName + " " + a.User.LastName,
                    Date = a.Date,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut,
                    Status = a.CheckIn.HasValue && a.CheckOut.HasValue
                             ? "Present"
                             : a.CheckIn.HasValue ? "Incomplete" : "Absent"
                }).ToList();

            return View(attendanceData);
        }





        [HttpGet]
        public IActionResult SummaryReport()
        {
            var users = _context.Users.Include(u => u.AttendanceUsers).ToList();

            var summary = users.Select(user =>
            {
                var attendances = user.AttendanceUsers;

                return new AttendanceSummaryViewModel
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PresentDays = attendances.Count(a => a.CheckIn != null && a.CheckOut != null),
                    IncompleteDays = attendances.Count(a => a.CheckIn != null && a.CheckOut == null),
                    AbsentDays = attendances.Count(a => a.CheckIn == null && a.CheckOut == null),
                    TotalHours = attendances.Sum(a => (double)(a.WorkHours ?? 0))
                };
            }).ToList();

            return View(summary);
        }


        public IActionResult ExportSummaryPdf()
        {
            var users = _context.Users.Include(u => u.AttendanceUsers).ToList();

            var summary = users.Select(user =>
            {
                var attendances = user.AttendanceUsers;

                return new AttendanceSummaryViewModel
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PresentDays = attendances.Count(a => a.CheckIn != null && a.CheckOut != null),
                    IncompleteDays = attendances.Count(a => a.CheckIn != null && a.CheckOut == null),
                    AbsentDays = attendances.Count(a => a.CheckIn == null && a.CheckOut == null),
                    TotalHours = attendances.Sum(a => (double)(a.WorkHours ?? 0))
                };
            }).ToList();

            // بناء HTML يدويًا
            var html = new StringBuilder();
            html.AppendLine("<h2>Attendance Summary Report</h2>");
            html.AppendLine("<table border='1' cellspacing='0' cellpadding='5' style='width:100%; font-family:sans-serif;'>");
            html.AppendLine("<thead><tr style='background-color:#f2f2f2;'><th>#</th><th>Employee</th><th>Present</th><th>Incomplete</th><th>Absent</th><th>Total Hours</th></tr></thead>");
            html.AppendLine("<tbody>");

            for (int i = 0; i < summary.Count; i++)
            {
                var item = summary[i];
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{i + 1}</td>");
                html.AppendLine($"<td>{item.FullName}</td>");
                html.AppendLine($"<td>{item.PresentDays}</td>");
                html.AppendLine($"<td>{item.IncompleteDays}</td>");
                html.AppendLine($"<td>{item.AbsentDays}</td>");
                html.AppendLine($"<td>{item.TotalHours}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</tbody></table>");

            // توليد PDF من HTML
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(html.ToString());

            byte[] pdf = doc.Save();
            doc.Close();

            return File(pdf, "application/pdf", "AttendanceSummary.pdf");
        }













    }

}
