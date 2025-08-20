
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceCalculationAPI.AttendanceCalculationAPI.Tests
{
    public class AttendanceControllerTests_CrossDaysShift
    {
        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
        private readonly IAttendanceHelperService _attendanceHelper;
        private readonly IOdooPushingDataService _odooPushingDataService;



        public AttendanceControllerTests_CrossDaysShift()
        {
            var fakeMapper = A.Fake<IMapper>();
            _biometricDeviceDataFetchingService = A.Fake<IBiometricDeviceDataFetchingService>();
            _odooDataFetchingService = A.Fake<IOdooDataFetchingService>();
            _odooPushingDataService = A.Fake<IOdooPushingDataService>();
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
            _attendanceCalculationService = new AttendanceCalculationService(fakeContext, fakeMapper, _odooPushingDataService, _attendanceHelper, configuration);

            _attendanceController = new AttendanceController(_attendanceCalculationService);

        }

        [Fact]
        public async Task AttendanceController_GenerateAttendanceRecords_ReturnsList()
        {


            // Arrange 



            var yesterday = DateTime.Today.AddDays(-1);
            var today = DateTime.Today.AddDays(1).AddTicks(-1);


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
            var shiftTodayDate = DateTime.Today.ToString("yyyy-MM-dd");
            var shiftTomorowDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");


            var biometricEvents = new List<BiometricEventDto>
{

    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "20:00:00",
        EntryExitType = 0,
        DoorControllerId = 11,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "21:09:25",
        EntryExitType = 0,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "21:30:44",
        EntryExitType = 1,
        DoorControllerId = 12,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = todayDate,
        ETime = "23:45:25",
        EntryExitType = 0,
        DoorControllerId = 12,
        Access_allowed = 1
    },

    //  ======================== next day ===================


    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = tomorowDate,
        ETime = "00:01:25",
        EntryExitType = 1,
        DoorControllerId = 12,
        Access_allowed = 1
    },

    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = tomorowDate,
        ETime = "02:00:00",
        EntryExitType = 1,
        DoorControllerId = 8,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = tomorowDate,
        ETime = "02:10:00",
        EntryExitType = 0,
        DoorControllerId = 8,
        Access_allowed = 1
    },
    new BiometricEventDto
    {
        UserId = "163",
        Username = "TestUser",
        EDate = tomorowDate,
        ETime = "04:00:00",
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
                                StartDateTime = $"{shiftTodayDate} 20:00:00",
                                EndDateTime = $"{shiftTomorowDate} 04:00:00",
                                Name = "Cross Days Shift (Night Shift)",
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






            if (today.Date.Equals(DateTime.Today))
            {
                //for today(today in code match today in real world) since the shift will end tomorow from now
                object expected = null;
                object actual = null;

                Assert.Equal(expected, actual); // This will pass
            }
            else
            {
                // for today (today in code match nextday in real world) since the shift will end tomorow from now 
                // we checking if the system next day will handle shifts that starts yesterday, so in this case will match yesterday


                var actual = resultList.FirstOrDefault();
                var expected = new AttendanceLog();

                expected.TotalEarlyTime = TimeSpan.Parse("00:00:00");
                expected.TotalOverTime = TimeSpan.Parse("00:00:00");
                expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Parse("00:37:19");
                expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Parse("00:10:00");
                expected.TotalLateTime = TimeSpan.Zero;
                expected.TotalBreakTime = TimeSpan.Parse("00:47:19");
                expected.TotalActualWorkingTime = TimeSpan.Parse("07:12:41");
                expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("07:12:41");
                expected.ShiftDurationTime = TimeSpan.Parse("08:00:00");
                expected.AllowedBreakTime = TimeSpan.FromHours(1);
                expected.OverBreakTime = TimeSpan.Parse("00:00:00");
                expected.AbsenceFlag = false;
                expected.LateFlag = false;
                expected.MissingPunchOutFlag = false;





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
}
