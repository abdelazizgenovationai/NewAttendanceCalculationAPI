using Newtonsoft.Json;

namespace NewAttendanceCalculationAPI.Models.Attendance
{
    public class BiometricDeviceDto
    {
        [JsonProperty("event-ta-date")]
        public List<BiometricEventDto> BiometricEventsList { get; set; }
    }
}
