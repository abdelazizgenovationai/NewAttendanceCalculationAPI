using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Controllers;
using NewAttendanceCalculationAPI.EntityFramework;
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
    public class AttendanceControllerTests_ShiftWithHoliday
    {
        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
            private readonly IOdooPushingDataService _odooPushingDataService;

        private readonly IAttendanceHelperService _attendanceHelper;



        public AttendanceControllerTests_ShiftWithHoliday()
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

            var request = new OdooEmployeeRequest
            {
                DateFrom = yesterday.ToString("yyyy-MM-dd"),
                DateTo = today.ToString("yyyy-MM-dd"),
                EmployeeId = null
            };

            var todayDate = DateTime.Today.ToString("dd/MM/yyyy");
            var shiftTodayDate = DateTime.Today.ToString("yyyy-MM-dd");

            var biometricEvents = new List<BiometricEventDto>
                                    {
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "08:00:00",
                                        EntryExitType = 0, // Punch In
                                        DoorControllerId = 11, // Attendance
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "09:09:25",
                                        EntryExitType = 0, // Punch In
                                        DoorControllerId = 12, // EntertainmentRoom
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "09:30:44",
                                        EntryExitType = 1, // Punch Out
                                        DoorControllerId = 12, // EntertainmentRoom
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "13:01:25",
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


                Calendar =new List<CalendarDto>(), // Empty as per JSON
                Shift = new List<ShiftDto>
                       {
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


            var emlpoyeeFullRecords = new List<FinalEmployeeResultDto> {
                new FinalEmployeeResultDto
            {
                EmployeeId = 163,
                EmployeeUniqueNumber = "163",
                Calendar = employeeoOdooList.FirstOrDefault()?.Calendar,
                Shift = employeeoOdooList.FirstOrDefault()?.Shift,
                BiometricEventList = biometricEvents
            }
            };

            A.CallTo(() => _odooDataFetchingService.GetEmployeeFromOdooAsync(request)).Returns(new OdooEmployeeResponse { Data = employeeoOdooList });
            A.CallTo(() => _biometricDeviceDataFetchingService.GetBiometricEventsByDateAsync(yesterday, today)).Returns(biometricEvents);


            


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
                BiometricEvents = biometricEvents,
                Employees = employeeoOdooList
            };

            A.CallTo(() => _attendanceHelper.GetIncludedEmployeesAndBiometricEventsAsync(A<DateTime>.Ignored, A<DateTime>.Ignored))

           .Returns(Task.FromResult(getIncludedEmployeesAndBiometricEventsAsyncResult));




            A.CallTo(() => _attendanceHelper.GetFullAttendanceInfo(employeeoOdooList, biometricEvents)).Returns(emlpoyeeFullRecords);




            var actionResult = await _attendanceController.GenerateAttendanceRecords();

            // Assert: Ensure the result is an OkObjectResult
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);  // Ensure it's not null

            // Extract the value from the OkObjectResult
            var resultList = okResult.Value as List<AttendanceLog>;
            Assert.NotNull(resultList);  // Ensure that the result list is not null

            var actual = resultList.FirstOrDefault();

            var expected = new AttendanceLog();



            expected.TotalEarlyTime = TimeSpan.Zero;
            expected.TotalOverTime = TimeSpan.Parse("00:03:30");
            expected.TotalSpentTimeInEntertainmentRoom = TimeSpan.Parse("01:05:19");
            expected.TotalSpentTimeOutSideCompanyDuringWorkingHours = TimeSpan.Zero;
            expected.TotalLateTime = TimeSpan.Zero;
            expected.TotalBreakTime = TimeSpan.Parse("01:05:19");
            expected.TotalActualWorkingTime = TimeSpan.Parse("07:58:11");
            expected.TotalTimeWorkedWithMeetings = TimeSpan.Parse("07:58:11");
            expected.ShiftDurationTime = TimeSpan.Parse("09:00:00");
            expected.AllowedBreakTime = TimeSpan.FromHours(1);
            expected.OverBreakTime = TimeSpan.Parse("00:05:19");
            expected.AbsenceFlag = false;
            expected.LateFlag = false;
            expected.MissingPunchOutFlag = false;


            //if( DateTime.Parse(holidayResult.Data.FirstOrDefault().DateFrom).Date.Equals(DateTime.Today.Date))
            //{
            //expected.HolidayDuration=TimeSpan.Parse("09:00:00");
            //}
            //else
            //{
            //            expected.HolidayDuration=TimeSpan.Parse("00:00:00");

            //}
            






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
            //Assert.Equal(expected.HolidayDuration, actual.HolidayDuration);




        }

    }
}
