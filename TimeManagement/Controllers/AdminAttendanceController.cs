using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }

}
