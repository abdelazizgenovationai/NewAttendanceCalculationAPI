using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services
{
    public interface IAttendanceHelperService
    {
         public List<FinalEmployeeResultDto> GetFullAttendanceInfo(List<OdooEmployeeDto> odooList, List<BiometricEventDto> biometricEvents);
        public AttendanceLog GetAttendanceLog(FinalEmployeeResultDto input);
        public Task<IncludedEmployeeData> GetIncludedEmployeesAndBiometricEventsAsync(DateTime yesterday, DateTime today);


    }
}
