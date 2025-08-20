namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto
{
    public class CheckoutDto
    {
        public TimeSpan Time { get; set; }
        public bool Actual { get; set; }
        public string Method { get; set; }
        public string Note { get; set; }

    }
}
