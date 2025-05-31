namespace TimeManagement.ViewModels
{
    public class AdminAttendanceViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateOnly Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } // present / absent / incomplete
    }

}
