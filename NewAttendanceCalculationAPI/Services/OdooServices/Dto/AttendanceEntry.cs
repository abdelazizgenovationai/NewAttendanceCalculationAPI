namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class AttendanceEntry
    {
        public int EmployeeId { get; set; }
        public string Date { get; set; }
        public bool IsCheckIn { get; set; }
        public bool IsAccepted { get; set; }
        public string EmployeeUniqueNumber { get; set; }
    }
}
