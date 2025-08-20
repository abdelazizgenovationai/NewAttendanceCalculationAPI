namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto
{
    public class MainUserActivity
    {
        public string Event { get; set; }
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }

        public TimeSpan Duration { get; set; }
        public string Color { get; set; }

        public string Note { get; set; }

        public List<SubUserActivity> Children { get; set; }

    }
}
