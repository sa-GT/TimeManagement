using TimeManagement.Models;

namespace TimeManagement.ViewModels
{
    public class AttendanceViewModel
    {
        public List<Attendance> Records { get; set; } = new();
        public bool CanCheckIn { get; set; }
        public bool CanCheckOut { get; set; }
        public Attendance? TodayRecord { get; set; }
    }
}
