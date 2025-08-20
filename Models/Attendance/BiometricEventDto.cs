using Newtonsoft.Json;

namespace NewAttendanceCalculationAPI.Models.Attendance
{
    public class BiometricEventDto
    {
        [JsonProperty("UserId")]
        public string EmployeeNumber { get; set; }
        [JsonProperty("Username")]
        public string EmployeeName { get; set; }
        [JsonProperty("EDate")]
        public string EventDate { get; set; }
        [JsonProperty("ETime")]
        public string EventTime { get; set; }
        [JsonProperty("EntryExitType")]
        public int IsPunchIn { get; set; }
        [JsonProperty("Access_allowed")]
        public int IsAllowed { get; set; }
        public int DoorControllerId { get; set; }
    }
}
