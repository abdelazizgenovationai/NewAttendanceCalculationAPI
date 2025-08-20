using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Controllers;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceCalculationAPI.AttendanceCalculationAPI.Tests
{
    public class AttendanceControllerTests_CrossDaysBusinessLeave
    {

        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
          private readonly IOdooPushingDataService _odooPushingDataService;

        private readonly IAttendanceHelperService _attendanceHelper;



        public AttendanceControllerTests_CrossDaysBusinessLeave()
        {
            var fakeMapper = A.Fake<IMapper>();
            _biometricDeviceDataFetchingService = A.Fake<IBiometricDeviceDataFetchingService>();
            _odooDataFetchingService = A.Fake<IOdooDataFetchingService>();
            _odooPushingDataService= A.Fake<IOdooPushingDataService>();
            _attendanceHelper = A.Fake<IAttendanceHelperService>();
            // Create an in-memory configuration instead of using FakeItEasy
            var inMemorySettings = new Dictionary<string, string>
            {
                { "AcceptedLateTimeInMinutes", "0" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

        



       var options = new DbContextOptionsBuilder<HRSystemServiceContext>()
    .UseInMemoryDatabase(databaseName: "TestDB")
    .Options;

var fakeContext = new HRSystemServiceContext(options);
            _attendanceCalculationService = new AttendanceCalculationService(fakeContext,fakeMapper,_odooPushingDataService, _attendanceHelper, configuration);

            _attendanceController = new AttendanceController(_attendanceCalculationService);

        }
        [Fact]
        public async Task AttendanceController_GenerateAttendanceRecords_ReturnsList()
        {


            // Arrange 



            var yesterday = DateTime.Today.AddDays(-1);
            var today = DateTime.Today.AddDays(1).AddTicks(-1);

            //today = DateTime.Today.AddDays(-1);
            //yesterday = DateTime.Today.AddDays(-2);


            var request = new OdooEmployeeRequest
            {
                DateFrom = yesterday.ToString("yyyy-MM-dd"),
                DateTo = today.ToString("yyyy-MM-dd"),
                EmployeeId = null
            };

            var todayDate = DateTime.Today.ToString("dd/MM/yyyy");

            var shiftYesterdayDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            var shiftTodayDate = DateTime.Today.ToString("yyyy-MM-dd");
            var shiftTomorowDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");


            var biometricEvents = new List<BiometricEventDto>
                                    {
                                      new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "12:01:25",
                                        EntryExitType = 0, // Punch In
                                        DoorControllerId = 11, // Attendance
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "13:00:00",
                                        EntryExitType = 0, // Punch In
                                        DoorControllerId = 12, // EntertainmentRoom
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "13:45:25",
                                        EntryExitType = 1, // Punch Out
                                        DoorControllerId = 12, // EntertainmentRoom
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "17:03:30",
                                        EntryExitType = 1, // Punch Out
                                        DoorControllerId = 8, // InformationTechnologyMain
                                        Access_allowed = 1
                                    }
                                };
            var employeeoOdooList = new List<OdooEmployeeDto>{
                    new OdooEmployeeDto
                {
                EmployeeId = 163,
                EmployeeUniqueNumber = "163",



                Calendar =new List<CalendarDto>(){

                     new CalendarDto
                               {
                               DateFrom= $"{shiftYesterdayDate} 08:00:00",
                               DateTo=$"{shiftYesterdayDate} 17:00:00",
                               Duration="9 hours",
                               TypeId=CalendarType.BusinessLeave.Id.ToString(),
                               TypeName=CalendarType.BusinessLeave.Name
                                },
                      new CalendarDto
                               {
                               DateFrom= $"{shiftTodayDate} 08:00:00",
                               DateTo=$"{shiftTodayDate} 12:00:00",
                               Duration="4 hours",
                               TypeId=CalendarType.BusinessLeave.Id.ToString(),
                               TypeName=CalendarType.BusinessLeave.Name
                                }
                       },
                Shift = new List<ShiftDto>
                       {
                    new ShiftDto
                               {
                                StartDateTime = $"{shiftYesterdayDate} 08:00:00",
                                EndDateTime = $"{shiftYesterdayDate} 17:00:00",
                                Name = "Regular Shift",
                                Id = 2
                                },
                    new ShiftDto
                               {
                                StartDateTime = $"{shiftTodayDate} 08:00:00",
                                EndDateTime = $"{shiftTodayDate} 17:00:00",
                                Name = "Regular Shift",
                                Id = 2
                                }
                       }
                 }
                                                           };


            var filteredBiometricEvents = biometricEvents.Where(x =>
           {

               var eventDate = AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime).Date;
               var todayDate = today.Date;
               var yesterday1 = yesterday.Date;

               return eventDate.Equals(todayDate) || eventDate.Equals(yesterday1);
           }).ToList();


            var filteredShifts = employeeoOdooList?.FirstOrDefault()?.Shift.Where(y => DateTime.Parse(y.StartDateTime).Date <= today.Date && DateTime.Parse(y.StartDateTime).Date >= yesterday.Date).ToList();


            var emlpoyeeFullRecords = new List<FinalEmployeeResultDto> {
                new FinalEmployeeResultDto
            {
                EmployeeId = 163,
                EmployeeUniqueNumber = "163",
                Calendar = employeeoOdooList.FirstOrDefault()?.Calendar,
                Shift = filteredShifts,
                BiometricEventList = filteredBiometricEvents
            }
            };

            A.CallTo(() => _odooDataFetchingService.GetEmployeeFromOdooAsync(request)).Returns(new OdooEmployeeResponse { Data = employeeoOdooList });
            A.CallTo(() => _biometricDeviceDataFetchingService.GetBiometricEventsByDateAsync(yesterday, today)).Returns(filteredBiometricEvents);

                  var holidayResult = new GetHolidayResponse
      {
          Status = 200,
          Msg = "Success",
          Data = new List<HolidayData>
          {
          new HolidayData
          {
              DateFrom = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
              DateTo = DateTime.Today.AddDays(3).AddTicks(-1).ToString("yyyy-MM-dd"),
              Name = "Eid Al adha"
          }
          }
      };

      A.CallTo(() => _odooDataFetchingService.GetHolidayAsync(A<GetHolidayRequest>.Ignored)).Returns(holidayResult);


            var getIncludedEmployeesAndBiometricEventsAsyncResult = new IncludedEmployeeData
            {
                BiometricEvents = filteredBiometricEvents,
                Employees = employeeoOdooList
            };

            A.CallTo(() => _attendanceHelper.GetIncludedEmployeesAndBiometricEventsAsync(A<DateTime>.Ignored, A<DateTime>.Ignored))

           .Returns(Task.FromResult(getIncludedEmployeesAndBiometricEventsAsyncResult));



            var filteredEmlpoyeeFullRecords = emlpoyeeFullRecords.Where(x =>

                x.Shift != null && x.BiometricEventList != null &&
                (x.Shift.OnlyShiftsThatStartTodayAndEndToday() || x.Shift.OnlyShiftsThatStartYesterdayAndEndToday()) &&

                x.BiometricEventList.Any(y => AttendanceExtensions.ConvertStringToDateTime(y.EDate, y.ETime).Date.Equals(today.Date))


                ).ToList();

            A.CallTo(() => _attendanceHelper.GetFullAttendanceInfo(employeeoOdooList, filteredBiometricEvents)).Returns(filteredEmlpoyeeFullRecords);




            var actionResult = await _attendanceController.GenerateAttendanceRecords();

            // Assert: Ensure the result is an OkObjectResult
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);  // Ensure it's not null

            // Extract the value from the OkObjectResult
            var resultList = okResult.Value as List<AttendanceLog>;
            Assert.NotNull(resultList);  // Ensure that the result list is not null

            var actual = resultList.FirstOrDefault();

            var expected = new AttendanceLog();


            // simulating where date is yesterday (full day business leave)
            if (today.Date.Equals(DateTime.Today.AddDays(-1).Date))
            {
                expected.TotalEarlyTime = TimeSpan.Zero;
                expected.TotalOverTime = TimeSpan.Zero;
                expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Zero;
                expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Parse("09:00:00");
                expected.TotalLateTime = TimeSpan.Zero;
                expected.TotalBreakTime = TimeSpan.Zero;
                expected.TotalActualWorkingTime = TimeSpan.Zero;
                expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("09:00:00");
                expected.ShiftDurationTime = TimeSpan.Parse("09:00:00");
                expected.AllowedBreakTime = TimeSpan.FromHours(1);
                expected.OverBreakTime = TimeSpan.Zero;
                expected.AbsenceFlag = true;
                expected.LateFlag = false;
                expected.MissingPunchOutFlag = true;
                expected.BusinessLeaveDuration=TimeSpan.Parse("09:00:00");
            }
            else
            {
                expected.TotalEarlyTime = TimeSpan.Zero;
                expected.TotalOverTime = TimeSpan.Parse("00:03:30");
                expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Parse("00:45:25");
                expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Parse("04:01:25");
                expected.TotalLateTime = TimeSpan.Parse("04:01:25");
                expected.TotalBreakTime = TimeSpan.Parse("00:46:50");// 04:16:40
                expected.TotalActualWorkingTime = TimeSpan.Parse("04:16:40");
                expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("08:16:40");
                expected.ShiftDurationTime = TimeSpan.Parse("09:00:00");
                expected.AllowedBreakTime = TimeSpan.FromHours(1);
                expected.OverBreakTime = TimeSpan.Zero;
                expected.AbsenceFlag = false;
                expected.LateFlag = true;
                expected.MissingPunchOutFlag = false;
                expected.BusinessLeaveDuration=TimeSpan.Parse("04:00:00");
            }







            Assert.Equal(expected.TotalEarlyTime, actual.TotalEarlyTime);
            Assert.Equal(expected.TotalOverTime, actual.TotalOverTime);
            Assert.Equal(expected.TotalSpentTimeInEntertainmentRoom, actual.TotalSpentTimeInEntertainmentRoom);
            Assert.Equal(expected.TotalSpentTimeOutSideCompanyDuringWorkingHours, actual.TotalSpentTimeOutSideCompanyDuringWorkingHours);
            Assert.Equal(expected.TotalLateTime, actual.TotalLateTime);
            Assert.Equal(expected.TotalBreakTime, actual.TotalBreakTime);
            Assert.Equal(expected.TotalActualWorkingTime, actual.TotalActualWorkingTime);
            Assert.Equal(expected.TotalTimeWorkedWithMeetings, actual.TotalTimeWorkedWithMeetings);
            Assert.Equal(expected.ShiftDurationTime, actual.ShiftDurationTime);
            Assert.Equal(expected.AllowedBreakTime, actual.AllowedBreakTime);
            Assert.Equal(expected.OverBreakTime, actual.OverBreakTime);
            Assert.Equal(expected.AbsenceFlag, actual.AbsenceFlag);
            Assert.Equal(expected.LateFlag, actual.LateFlag);
            Assert.Equal(expected.MissingPunchOutFlag, actual.MissingPunchOutFlag);
            Assert.Equal(expected.BusinessLeaveDuration, actual.BusinessLeaveDuration);




        }


    }
}
