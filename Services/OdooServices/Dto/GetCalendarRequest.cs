namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class GetCalendarRequest
    {
         public int? EmployeeId { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
    }
}
