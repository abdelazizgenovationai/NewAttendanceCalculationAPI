
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto
{
    public class FinalEmployeeResultDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeUniqueNumber { get; set; }
        public List<CalendarDto>? Calendar { get; set; } = new();
        public List<ShiftDto>? Shift { get; set; }
        public List<BiometricEventDto>? BiometricEventList { get; set; }
    }
}
