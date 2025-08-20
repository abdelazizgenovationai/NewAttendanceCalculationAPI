using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto
{
    public class BiometricEventWrapper
    {
        [JsonPropertyName("event-ta-date")]
        public List<BiometricEventDto> Events { get; set; } = new();
    }
}
