using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;

namespace NewAttendanceCalculationAPI.Services.BiometricDeviceServices
{
     public interface IBiometricDeviceDataFetchingService
    {

        public  Task<List<BiometricEventDto>> GetBiometricEventsByDateAsync(DateTime fromDate, DateTime toDate);
    }
}
