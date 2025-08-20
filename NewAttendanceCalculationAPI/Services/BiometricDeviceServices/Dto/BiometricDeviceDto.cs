using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto
{
    public class BiometricDeviceDto
    {
         [JsonPropertyName("event-ta-date")]
        public List<BiometricEventDto> BiometricEventsList { get; set; } = new List<BiometricEventDto>();

    }
}
