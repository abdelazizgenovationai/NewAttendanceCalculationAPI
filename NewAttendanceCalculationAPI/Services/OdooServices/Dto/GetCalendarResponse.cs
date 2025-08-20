namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
      public class GetCalendarResponse
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public List<CalendarEntry> Data { get; set; }
    }
}
