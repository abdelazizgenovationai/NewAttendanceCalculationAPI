using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class OdooEmployeeResponse
    {


        [JsonPropertyName("data")]
        public List<OdooEmployeeDto> Data { get; set; } = new();
    }
}
