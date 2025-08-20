using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using System.Collections.Generic;
using System.Globalization;


namespace NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices
{
    public class AttendanceCalculationService : IAttendanceCalculationService
    {

        private HRSystemServiceContext _context;
        private readonly IMapper _mapper;
        private readonly IOdooPushingDataService _odooPushingDataService; //IOdooPushingDataService
        private readonly IAttendanceHelperService _attendanceHelper;
        private readonly IConfiguration _configuration;
        public AttendanceCalculationService(
            HRSystemServiceContext contenxt,

            IMapper mapper,
            IOdooPushingDataService odooPushingDataService,

            IAttendanceHelperService attendanceHelper,
            IConfiguration configuration
            )
        {
            _context = contenxt;
            _mapper = mapper;
            _odooPushingDataService = odooPushingDataService;
            _attendanceHelper = attendanceHelper;
            _configuration = configuration;
        }



        //it will return only if the user has data on both sources: Odoo and Biometric device
        public async Task<List<AttendanceLog>> GetAttendanceLogs()
        {


            try
            {



                //var holidays = await _odppService.GetHolidayAsync(new GetHolidayRequest { DateFrom=null,DateTo=null});

                //#if DEBUG
                //                var yesterday = DateTime.Parse("2025-05-05 00:00:00");
                //                var today = DateTime.Parse("2025-05-06 23:59:59.999");

                //#else
                var yesterday = DateTime.Today.AddDays(-1);
                var today = DateTime.Today;

                //#endif




                var neededEmployeeData = await _attendanceHelper.GetIncludedEmployeesAndBiometricEventsAsync(yesterday, today);


                List<FinalEmployeeResultDto> emlpoyeeFullRecords = _attendanceHelper.GetFullAttendanceInfo(neededEmployeeData.Employees, neededEmployeeData.BiometricEvents);


                var result = emlpoyeeFullRecords.Where(x => x.Shift.OnlyShiftsThatStartTodayAndEndToday() || x.Shift.OnlyShiftsThatStartYesterdayAndEndToday()

                ).Select((employeeFullRecord) => _attendanceHelper.GetAttendanceLog(employeeFullRecord)).ToList();



                // --- Check which logs already exist in the DB ---
                var employeeIds = result.Select(r => r.EmployeeId).Distinct().ToList();

                var existingLogs = await _context.AttendanceLogs
                    .Where(a => employeeIds.Contains(a.EmployeeId)
                                && a.CreatedDate >= yesterday
                                && a.CreatedDate <= today)
                    .Select(a => new { a.EmployeeId, a.CreatedDate })
                    .ToListAsync();

                // Filter logs that are NOT already in the database
                var logsToSendToOdoo = result
                    .Where(r => !existingLogs.Any(e => e.EmployeeId == r.EmployeeId
                                                      && e.CreatedDate.Date == r.CreatedDate.Date))
                    .ToList();

                if (logsToSendToOdoo.Any())
                {
                    // Map to Odoo DTO and push
                    var toInsert = _mapper.Map<List<AttendanceLogForOdoo>>(logsToSendToOdoo);
                    var odooInsertionResult = await _odooPushingDataService.InsertCalculatedAttendanceAsync(toInsert);

                    if (odooInsertionResult)
                    {
                        // Insert newly sent logs into DB
                        await _context.AttendanceLogs.AddRangeAsync(logsToSendToOdoo);
                        await _context.SaveChangesAsync();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        public async Task<UserActivitySummaryDto> GetDashbaordData(int UserId, DateTime DateFrom, DateTime DateTo)
        {
            try
            {


                List<int> mainDoors = new List<int>

                    {
                       (int)AccessControlDoor.EntertainmentRoom,
                       (int)AccessControlDoor.ReceptionMain,
                       (int)AccessControlDoor.Attendance,
                       (int) AccessControlDoor.InformationTechnology,
                       (int)AccessControlDoor.ElectronicPayments


                    };


                const int punchInAction = (int)EmployeeAction.PunchIn;
                const int punchOutAction = (int)EmployeeAction.PunchOut;

                var neededEmployeeData = await _attendanceHelper.GetIncludedEmployeesAndBiometricEventsAsync(DateFrom, DateTo);

                List<FinalEmployeeResultDto> emlpoyeeFullRecords = _attendanceHelper.GetFullAttendanceInfo(neededEmployeeData.Employees, neededEmployeeData.BiometricEvents);


                var result = emlpoyeeFullRecords?.Select(
                    (x) =>
                    {


                        var shiftStartDateTime = x.Shift.FirstOrDefault()?.StartDateTime;

                        var lateFlag = x.BiometricEventList.LateFlag(DateTime.Parse(shiftStartDateTime), TimeSpan.FromMinutes(_configuration.GetValue<int>("AcceptedLateTimeInMinutes")));


                        var checkIn = new CheckInDto
                        {
                            Time = x.BiometricEventList.FirstPunchIn().TimeOfDay,
                            Status = lateFlag ? "Late" : "On-Time",
                            Color = lateFlag ? "red" : "green"
                        };

                        var checkOut = new CheckoutDto
                        {
                            Time = x.BiometricEventList.LastPunchOut().TimeOfDay,
                            Actual = false,
                            Method = "Real",
                            Note = "No note"
                        };




                        var userSubActivity = x.BiometricEventList
                        .Where(x => x.DoorControllerId == (int)AccessControlDoor.EntertainmentRoom)?
                        .OrderBy(e => DateTime.ParseExact($"{e.EDate} {e.ETime}", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();


                        var subActivityResult = new List<SubUserActivity>();


                        for (int i = 0; i <= userSubActivity?.Count - 1; i++)
                        {
                            var result = new SubUserActivity();
                            if (userSubActivity?[i].EntryExitType == punchInAction && mainDoors.Contains(userSubActivity[i].DoorControllerId))
                            {
                                result.Event = "EntertainmentRoom";

                                result.TimeIn = TimeSpan.Parse(userSubActivity[i].ETime);


                                if (i != userSubActivity?.Count - 1 && userSubActivity[i + 1].EntryExitType == punchOutAction)
                                {
                                    result.TimeOut = TimeSpan.Parse(userSubActivity[i + 1].ETime);
                                    result.Duration = TimeSpan.Parse(userSubActivity[i + 1].ETime) - TimeSpan.Parse(userSubActivity[i].ETime);
                                    result.Color = result.Duration <= TimeSpan.FromHours(1) ? "green" : "red";
                                }
                                else
                                {


                                    var timeout = x.BiometricEventList.Where(y => DateTime.Parse(y.ETime) > DateTime.Parse(userSubActivity[i].ETime)).FirstOrDefault()?.ETime ?? TimeSpan.Zero.ToString();


                                    result.TimeOut = TimeSpan.Parse(timeout);
                                    result.Duration = result.TimeOut - TimeSpan.Parse(userSubActivity[i].ETime);
                                    result.Color = "red";
                                }







                                subActivityResult.Add(result);

                            }


                        }













                        return new UserActivitySummaryDto
                        {
                            CheckIn = checkIn,
                            Activity = new List<MainUserActivity>
                    {
                        new MainUserActivity{
                        Event="Main Door Entry or First Detected Punch",
                        TimeIn= x.BiometricEventList.FirstPunchIn().TimeOfDay,
                        TimeOut=x.BiometricEventList.LastPunchOut().TimeOfDay,
                        Duration=x.BiometricEventList.LastPunchOut() - x.BiometricEventList.FirstPunchIn(),
                        Color= lateFlag ? "red" : "green",
                        Children=subActivityResult
                        }


                    },
                            CheckOutDto = checkOut

                        };


                    }).FirstOrDefault();


                return result;
            }
            catch (Exception ex)
            {
                throw;
            }


        }


    }

}
