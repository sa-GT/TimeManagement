namespace TimeManagement.ViewModels
{
    public class AttendanceSummaryViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int IncompleteDays { get; set; }
        public double TotalHours { get; set; }
    }

}
