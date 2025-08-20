using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    using System.Text.Json.Serialization;

    public class OdooEmployeeDto
    {
        [JsonPropertyName("employeeId")]
        public int EmployeeId { get; set; }

        [JsonPropertyName("employeeUniqueNumber")]
        public string EmployeeUniqueNumber { get; set; }

        [JsonPropertyName("calendar")]
        public List<CalendarDto> Calendar { get; set; } = new();

        [JsonPropertyName("shift")]
        public List<ShiftDto> Shift { get; set; }
    }

}
