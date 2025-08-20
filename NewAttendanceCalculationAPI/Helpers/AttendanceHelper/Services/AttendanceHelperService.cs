using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using Azure.Core;
using System.Configuration;
using System.Text.Json;

namespace NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services
{
     public class AttendanceHelperService: IAttendanceHelperService
    {
        private readonly IBiometricDeviceDataFetchingService _biometricDevice;
        private readonly IOdooDataFetchingService _odooService;
        private readonly IConfiguration _configuration;

        public AttendanceHelperService(
            IConfiguration configuration,
              IBiometricDeviceDataFetchingService biometricDevice,
            IOdooDataFetchingService odooService
            )
        {
            _configuration = configuration;
            _biometricDevice = biometricDevice;
            _odooService = odooService;
        }
        public List<FinalEmployeeResultDto> GetFullAttendanceInfo(List<OdooEmployeeDto> odooList, List<BiometricEventDto> biometricEvents)
        {
            var result = odooList
                    .Select(emp => new FinalEmployeeResultDto
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeUniqueNumber = "34", // Replace with actual logic
                        Calendar = emp.Calendar,
                        Shift = emp.Shift,
                        BiometricEventList = biometricEvents.Where(e => int.Parse(e.UserId) == emp.EmployeeId).ToList()
                    }).ToList();


            return result;
        }


        public AttendanceLog GetAttendanceLog(FinalEmployeeResultDto input)
        {
            var result = new AttendanceLog();



  
            DateTime shiftStartDateTime = input.Shift?.ShiftStartDateTime() ?? DateTime.MinValue;
            DateTime shiftEndDateTime = input.Shift?.ShiftEndDateTime() ?? DateTime.MinValue;

            var userBiometricEvents = input.BiometricEventList?.Where(y =>  /*removing left side zero*/ y.UserId.TrimStart('0') == input.EmployeeId.ToString()).ToList(); ;
            result.AllowedBreakTime = AttendanceLogStatisticsExtensions.AllowdBreakTime();
            result.FirstPunch = userBiometricEvents?.FirstPunch();
            result.LastPunch = userBiometricEvents?.LastPunch();
            result.OverBreakTime = userBiometricEvents?.OverBreakTime(shiftStartDateTime, shiftEndDateTime);
            result.ShiftDurationTime = shiftEndDateTime - shiftStartDateTime;

            result.TotalActualWorkingTime = userBiometricEvents?.TotalActualWorkingTime(shiftStartDateTime, shiftEndDateTime);

            result.TotalBreakTime = userBiometricEvents?.TotalBreakTime(shiftStartDateTime, shiftEndDateTime);
            result.TotalSpentTimeInEntertainmentRoom = userBiometricEvents?.TotalSpentTimeInSpecifiedRoom(new List<int> { (int)AccessControlDoor.EntertainmentRoom });
            result.TotalSpentTimeOutSideCompanyDuringWorkingHours = userBiometricEvents?.TotalTimeSpentOutsideCompany(shiftStartDateTime, shiftEndDateTime);
            result.EmployeeId = input.EmployeeId;
            result.UserName = userBiometricEvents?.First(y => y.UserId.TrimStart('0') == input.EmployeeId.ToString())?.Username ?? "";
            result.PunchInDateTime = userBiometricEvents?.FirstPunchIn();
            result.PunchInDate = userBiometricEvents?.FirstPunchIn().Date;
            result.PunchInTime = userBiometricEvents?.FirstPunchIn().TimeOfDay;
            result.PunchOutDateTime = userBiometricEvents?.LastPunchOut();
            result.PunchOutDate = userBiometricEvents?.LastPunchOut().Date;
            result.PunchOutTime = userBiometricEvents?.LastPunchOut().TimeOfDay;

            result.TotalEarlyTime = userBiometricEvents?.TotalEarlyTime(shiftStartDateTime);
            result.TotalOverTime = userBiometricEvents?.TotalOverTime(shiftEndDateTime, shiftStartDateTime);

            result.ShiftStartTime = shiftStartDateTime.TimeOfDay;
            result.ShiftEndTime = shiftEndDateTime.TimeOfDay;
            result.AbsenceFlag = (userBiometricEvents != null && userBiometricEvents.AbsenceFlag() && input.Shift  != null && !input.Shift.AbsenceFlag());// there is no biometric events and there is a shift list linked to his id from Odoo
            result.LateFlag = userBiometricEvents != null ? userBiometricEvents.LateFlag(shiftStartDateTime, TimeSpan.FromMinutes(_configuration.GetValue<int>("AcceptedLateTimeInMinutes"))):true;
            result.TotalLateTime = userBiometricEvents != null ? userBiometricEvents.TotalLateTime(shiftStartDateTime, TimeSpan.FromMinutes(_configuration.GetValue<int>("AcceptedLateTimeInMinutes"))): TimeSpan.Zero;
            result.MissingPunchOutFlag = userBiometricEvents != null && userBiometricEvents.MissingPunchOut();

            result.MeetingDuration = input.Calendar?.Where(c=>c.TypeName.Equals(CalendarType.Meeting.Name)).ToList().CalculateDurationForMeetings(userBiometricEvents, shiftStartDateTime, shiftEndDateTime);
            result.BusinessLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.BusinessLeave.Name, shiftStartDateTime, shiftEndDateTime);


            result.TotalTimeWorkedWithMeetings = result.MeetingDuration + result.BusinessLeaveDuration  + result.TotalActualWorkingTime; 


            result.AnnualLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.AnnualLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.SickLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.SickLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.CompassionateLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.CompassionateLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.AuthorizedUnpaidLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.AuthorizedUnpaidLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.UnauthorizedUnpaidLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.UnauthorizedUnpaidLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.HajjLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.HajjLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.BreakLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.BreakLeave.Name, shiftStartDateTime, shiftEndDateTime);
            result.MaternityLeaveDuration = input.Calendar?.CalculateDurationForType(CalendarType.MaternityLeave.Name, shiftStartDateTime, shiftEndDateTime);


            result.ResetNegativeTimeSpans();
            return result;




        }




        public async Task<IncludedEmployeeData> GetIncludedEmployeesAndBiometricEventsAsync(DateTime yesterday, DateTime today)
        {
            // Start of yesterday (00:00:00)
    var start = yesterday.Date;

    // End of today (23:59:59.999)
    var end = today.Date.AddDays(1).AddTicks(-1);


#if DEBUG

            var employeeListFromOdoo = await LoadFromJsonAsync<OdooEmployeeResponse>("C:\\Users\\ThinkPad\\Desktop\\Attendance Project Files\\Biometric-Data-Samples\\DataForTest\\employeesFromOdoo.json");
            var biometricData=  await LoadFromJsonAsync<BiometricEventWrapper>("C:\\Users\\ThinkPad\\Desktop\\Attendance Project Files\\Biometric-Data-Samples\\DataForTest\\biometricevents.json");
            var employeesBiometricEvents = biometricData.Events.
                Where(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) <= end && AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) >= start).ToList();


            //var test = biometricData.Events.GroupBy(x => new { x.Username , x.UserId }).Select(x=>new { 
            
            //Id=x.Key.UserId,
            //Key=x.Key.Username,
            //Count=x.Count(),

            //}).OrderByDescending(x=>x.Count).ToList();

#else
            var request = new OdooEmployeeRequest
            {
                DateFrom = yesterday.ToString("yyyy-MM-dd"),
                DateTo = today.ToString("yyyy-MM-dd"),
                EmployeeId = null
            };


            var employeeListFromOdoo = await _odooService.GetEmployeeFromOdooAsync(request);
            var employeesBiometricEvents = await _biometricDevice.GetBiometricEventsByDateAsync(yesterday, today);
#endif


            var odooEmployeeIds = employeeListFromOdoo.Data.Select(y => y.EmployeeId).ToList();

            var employeesBiometricEventsIds = employeesBiometricEvents.Select(x => int.Parse(x.UserId.TrimStart('0'))).ToList();


            var intersectedIds = employeesBiometricEventsIds.Intersect(odooEmployeeIds).ToList();

            var includedBiometricEventList = employeesBiometricEvents
                .Where(x => intersectedIds.Contains(int.Parse(x.UserId))).ToList();

            var includedEmployeeListFromOdoo = employeeListFromOdoo?.Data
                .Where(x => intersectedIds.Contains(x.EmployeeId)).ToList();


            return new IncludedEmployeeData
            {
                BiometricEvents = includedBiometricEventList,
                Employees = includedEmployeeListFromOdoo
            };
        }






        private async Task<T> LoadFromJsonAsync<T>(string filePath)
        {

            try
            {
                using var stream = File.OpenRead(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false // 👈 make it case-sensitive
                };
                return await JsonSerializer.DeserializeAsync<T>(stream, options);

            }
            catch (Exception ex)
            {
                throw;
            }

        }




    }
}
