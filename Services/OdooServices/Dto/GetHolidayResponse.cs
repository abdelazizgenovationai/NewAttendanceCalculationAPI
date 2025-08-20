namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class GetHolidayResponse
    {
           public int Status { get; set; }
        public string Msg { get; set; }
        public List<HolidayData> Data { get; set; }
    }
}
