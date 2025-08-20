using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
    public interface IOdooDataFetchingService
    {
        public Task<GetHolidayResponse?> GetHolidayAsync(GetHolidayRequest request);

        public Task<GetCalendarResponse?> GetCalendarAsync(GetCalendarRequest request);

        public Task<LoginResponse?> LoginAsync(LoginRequest request);

        public Task<GetAttendanceResponse?> GetAttendanceAsync(GetAttendanceRequest request);

        public Task<OdooEmployeeResponse?> GetEmployeeFromOdooAsync(OdooEmployeeRequest request);
    }
}
