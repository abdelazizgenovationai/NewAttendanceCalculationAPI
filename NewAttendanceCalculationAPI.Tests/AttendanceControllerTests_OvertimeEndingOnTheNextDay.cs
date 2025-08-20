using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Controllers;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services;
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
    public class AttendanceControllerTests_OvertimeEndingOnTheNextDay
    {


        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
        private readonly IAttendanceHelperService _attendanceHelper;
            private readonly IOdooPushingDataService _odooPushingDataService;



        public AttendanceControllerTests_OvertimeEndingOnTheNextDay()
        {
            var fakeMapper = A.Fake<IMapper>();
            _biometricDeviceDataFetchingService = A.Fake<IBiometricDeviceDataFetchingService>();
            _odooDataFetchingService = A.Fake<IOdooDataFetchingService>();
              _odooPushingDataService= A.Fake<IOdooPushingDataService>();
            _attendanceHelper = A.Fake<IAttendanceHelperService>();
            // Create an in-memory configuration instead of using FakeItEasy
            var inMemorySettings = new Dictionary<string, string>
            {
                { "AcceptedLateTimeInMinutes", "10" }
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



            // simulating tomorrow that tomorow became today (test how the data will be handled in tomorow)
            yesterday = DateTime.Today;
            today = DateTime.Today.AddDays(2).AddTicks(-1);





            var request = new OdooEmployeeRequest
            {
                DateFrom = yesterday.ToString("yyyy-MM-dd"),
                DateTo = today.ToString("yyyy-MM-dd"),
                EmployeeId = null
            };


            var todayDate = DateTime.Today.ToString("dd/MM/yyyy");
            var tomorowDate = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy");
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
        ETime = "13:38:03",
        EntryExitType = 0,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "13:42:02",
        EntryExitType = 1,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "14:00:00",
        EntryExitType = 0,
        DoorControllerId = 11,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "15:09:25",
        EntryExitType = 0,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "15:30:44",
        EntryExitType = 1,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "19:01:25",
        EntryExitType = 0,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "19:45:25",
        EntryExitType = 1,
        DoorControllerId = 12,
        Access_allowed = 1
    }
    
    // this is for next day, so it will not be fetched by GetBiometricEventsByDateAsync(yesterday, today)

    ,new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = tomorowDate,
        ETime = "02:03:30",
        EntryExitType = 1,
        DoorControllerId = 8,
        Access_allowed = 1
    },
    new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "13:38:03",
    EntryExitType = 0,
    DoorControllerId = 12,
    Access_allowed = 1
},
new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "13:42:02",
    EntryExitType = 1,
    DoorControllerId = 12,
    Access_allowed = 1
},
new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "14:00:00",
    EntryExitType = 0,
    DoorControllerId = 11,
    Access_allowed = 1
},
new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "15:09:25",
    EntryExitType = 0,
    DoorControllerId = 12,
    Access_allowed = 1
},
new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "15:30:44",
    EntryExitType = 1,
    DoorControllerId = 12,
    Access_allowed = 1
},
new BiometricEventDto
{
    UserId = "163",
    Username = "TestUser",
    EDate = tomorowDate,
    ETime = "21:00:00",
    EntryExitType = 1,
    DoorControllerId = 8,
    Access_allowed = 1
}

};



            var employeeoOdooList = new List<OdooEmployeeDto>{
                    new OdooEmployeeDto
                {
                EmployeeId = 163,
                EmployeeUniqueNumber = "163",


                Calendar =new List<CalendarDto>(), // Empty as per JSON
                Shift = new List<ShiftDto>
                       {
                    new ShiftDto
                               {
                                StartDateTime = $"{shiftYesterdayDate} 14:00:00",
                                EndDateTime = $"{shiftYesterdayDate} 21:00:00",
                                Name = "Evening Shift",
                                Id = 2
                                },
                    new ShiftDto
                               {
                                StartDateTime = $"{shiftTodayDate} 14:00:00",
                                EndDateTime = $"{shiftTodayDate} 21:00:00",
                                Name = "Evening Shift",
                                Id = 2
                                },
                    new ShiftDto
                               {
                                StartDateTime = $"{shiftTomorowDate} 14:00:00",
                                EndDateTime = $"{shiftTomorowDate} 21:00:00",
                                Name = "Evening Shift",
                                Id = 2
                                }
                       }
                 }
                                                           };



            var filteredBiometricEvents = biometricEvents.Where(x =>
            {

                var eventDate = AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime).Date;
                var todayDate = today.Date;

                return eventDate.Equals(todayDate);
            }).ToList();


            var filteredShifts = employeeoOdooList?.FirstOrDefault()?.Shift.Where(y => DateTime.Parse(y.StartDateTime).Date == today.Date).ToList();


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
            //   A.CallTo(() => _attendanceHelper.GetFullAttendanceInfo(employeeoOdooList, filteredBiometricEvents)).Returns(emlpoyeeFullRecords);




            var actionResult = await _attendanceController.GenerateAttendanceRecords();

            // Assert: Ensure the result is an OkObjectResult
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);  // Ensure it's not null

            // Extract the value from the OkObjectResult
            var resultList = okResult.Value as List<AttendanceLog>;
            Assert.NotNull(resultList);  // Ensure that the result list is not null

            var actual = resultList.FirstOrDefault();

            var expected = new AttendanceLog();




            //for today(today in code match today in real world) since the shift will end tomorow from now

            if (today.Date.Equals(DateTime.Today))
            {
            
            
            expected.TotalEarlyTime = TimeSpan.Parse("00:21:57");
            expected.TotalOverTime = TimeSpan.Parse("00:17:58");
            expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Parse("01:09:18");
            expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Zero;
            expected.TotalLateTime = TimeSpan.Zero;
            expected.TotalBreakTime = TimeSpan.Parse("01:09:18");
            expected.TotalActualWorkingTime = TimeSpan.Parse("04:58:04");
            expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("04:58:04");
            expected.ShiftDurationTime = TimeSpan.Parse("07:00:00");
            expected.AllowedBreakTime = TimeSpan.FromHours(1);
            expected.OverBreakTime = TimeSpan.Parse("00:09:18");
            expected.AbsenceFlag = false;
            expected.LateFlag = false;
            expected.MissingPunchOutFlag = true;
            }
            else
            {
                // for today (today in code match nextday in real world) since the shift will end tomorow from now 
                // we checking if the system next day will handle shifts that starts yesterday, so in this case will match yesterday


                expected.TotalEarlyTime = TimeSpan.Parse("00:21:57");
                expected.TotalOverTime = TimeSpan.Parse("02:21:28");
                expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Parse("00:25:18");
                expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Zero;
                expected.TotalLateTime = TimeSpan.Zero;
                expected.TotalBreakTime = TimeSpan.Parse("00:25:18");
                expected.TotalActualWorkingTime = TimeSpan.Parse("09:00:09");
                expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("09:00:09");
                expected.ShiftDurationTime = TimeSpan.Parse("07:00:00");
                expected.AllowedBreakTime = TimeSpan.FromHours(1);
                expected.OverBreakTime = TimeSpan.Parse("00:00:00");
                expected.AbsenceFlag = false;
                expected.LateFlag = false;
                expected.MissingPunchOutFlag = false;
            }


            





            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalEarlyTime.Value), AttendanceExtensions.StripMilliseconds(actual.TotalEarlyTime.Value));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalOverTime), AttendanceExtensions.StripMilliseconds(actual.TotalOverTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalSpentTimeInEntertainmentRoom), AttendanceExtensions.StripMilliseconds(actual.TotalSpentTimeInEntertainmentRoom));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalSpentTimeOutSideCompanyDuringWorkingHours), AttendanceExtensions.StripMilliseconds(actual.TotalSpentTimeOutSideCompanyDuringWorkingHours));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalLateTime), AttendanceExtensions.StripMilliseconds(actual.TotalLateTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalBreakTime), AttendanceExtensions.StripMilliseconds(actual.TotalBreakTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalActualWorkingTime), AttendanceExtensions.StripMilliseconds(actual.TotalActualWorkingTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.TotalTimeWorkedWithMeetings), AttendanceExtensions.StripMilliseconds(actual.TotalTimeWorkedWithMeetings));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.ShiftDurationTime), AttendanceExtensions.StripMilliseconds(actual.ShiftDurationTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.AllowedBreakTime), AttendanceExtensions.StripMilliseconds(actual.AllowedBreakTime));
            Assert.Equal(AttendanceExtensions.StripMilliseconds(expected.OverBreakTime), AttendanceExtensions.StripMilliseconds(actual.OverBreakTime));
            Assert.Equal(expected.AbsenceFlag, actual.AbsenceFlag);
            Assert.Equal(expected.LateFlag, actual.LateFlag);
            Assert.Equal(expected.MissingPunchOutFlag, actual.MissingPunchOutFlag);




        }
    }
}
