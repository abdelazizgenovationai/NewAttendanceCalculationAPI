namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class OdooEmployeeRequest
    {
              public int? EmployeeId { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
    }
}
