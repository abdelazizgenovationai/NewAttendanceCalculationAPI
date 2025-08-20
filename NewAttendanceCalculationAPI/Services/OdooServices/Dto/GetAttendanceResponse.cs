namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class GetAttendanceResponse
    {
           public int Status { get; set; }
        public string Msg { get; set; }
        public List<AttendanceEntry> Data { get; set; }
    }
}
