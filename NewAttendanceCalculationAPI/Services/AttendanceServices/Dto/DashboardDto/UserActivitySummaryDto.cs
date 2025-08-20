namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto
{
    public class UserActivitySummaryDto
    {
        public CheckInDto CheckIn { get; set; }
        public List<MainUserActivity> Activity { get; set; }

        public CheckoutDto CheckOutDto { get; set; }

    }
}
