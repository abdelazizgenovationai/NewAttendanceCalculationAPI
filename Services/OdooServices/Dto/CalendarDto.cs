using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class CalendarDto
    {
        [JsonPropertyName("dateFrom")]
        public string DateFrom { get; set; } = string.Empty;

        [JsonPropertyName("dateTo")]
        public string DateTo { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("TypeId")]
        public string TypeId { get; set; } = string.Empty;

        [JsonPropertyName("TypeName")]
        public string TypeName { get; set; } = string.Empty;
    }
}
