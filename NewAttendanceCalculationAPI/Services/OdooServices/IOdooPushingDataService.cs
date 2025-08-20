using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
    public interface IOdooPushingDataService
    {


         Task<bool> InsertCalculatedAttendanceAsync(List<AttendanceLogForOdoo> data);
    }
}
