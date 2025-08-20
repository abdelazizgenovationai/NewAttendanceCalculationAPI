using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class ShiftDto
    {
        [JsonPropertyName("StartDateTime")]
        public string StartDateTime { get; set; } = string.Empty;

        [JsonPropertyName("EndDateTime")]
        public string EndDateTime { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Id")]
        public int Id { get; set; }
    }
}
