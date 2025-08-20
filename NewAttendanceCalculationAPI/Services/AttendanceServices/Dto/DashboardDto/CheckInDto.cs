namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto
{
    public class CheckInDto
    {
        public TimeSpan Time { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
    }
}
