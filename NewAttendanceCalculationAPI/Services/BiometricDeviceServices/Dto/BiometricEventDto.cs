using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto
{
       public class BiometricEventDto
    {
        [Key]
        [JsonPropertyName("userid")]
        public string UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("edate")]
        public string EDate { get; set; }

        [JsonPropertyName("etime")]
        public string ETime { get; set; }

        [JsonPropertyName("entryexittype")]
        [JsonConverter(typeof(IntFromStringConverter))]
        public int EntryExitType { get; set; }

        [JsonPropertyName("access_allowed")]
        [JsonConverter(typeof(IntFromStringConverter))]
        public int Access_allowed { get; set; }

        [JsonPropertyName("doorcontrollerid")]
        [JsonConverter(typeof(IntFromStringConverter))]
        public int DoorControllerId { get; set; }
    }

    public class IntFromStringConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (int.TryParse(reader.GetString(), out int result))
                {
                    return result;
                }
                throw new JsonException($"Invalid integer value: {reader.GetString()}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            throw new JsonException("Invalid JSON token for integer conversion.");
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
