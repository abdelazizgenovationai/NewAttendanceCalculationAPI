using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices
{
    public interface IAttendanceCalculationService
    {

        public Task<List<AttendanceLog>> GetAttendanceLogs();

        public Task<UserActivitySummaryDto> GetDashbaordData(int UserId, DateTime DateFrom, DateTime DateTo);

    }
}
