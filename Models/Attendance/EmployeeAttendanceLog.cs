namespace NewAttendanceCalculationAPI.Models.Attendance
{
    public class EmployeeAttendanceLog
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string DoorName { get; set; }
        public bool IsPunchIn { get; set; }
        public DateTime Date { get; set; }
        public int Time { get; set; }
        public bool IsAccepted { get; set; }
        public int IsAllowed { get; set; }
        public string Note { get; set; }
    }
}
