namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto
{
    public class SubUserActivity
    {

        public string Event { get; set; }
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }
        public TimeSpan Duration { get; set; }
        public string Color { get; set; }
    }
}
