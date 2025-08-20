namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class CalendarEntry
    {
                public int EmployeeId { get; set; }
        public string EmployeeUniqueNumber { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Duration { get; set; }
        public int CalendarTypeId { get; set; }
        public string CalendarTypeName { get; set; }
    }
}
