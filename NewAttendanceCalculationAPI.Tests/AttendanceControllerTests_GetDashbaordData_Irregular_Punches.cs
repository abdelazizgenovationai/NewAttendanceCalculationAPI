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
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto.DashboardDto;
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
    public class AttendanceControllerTests_GetDashbaordData_Irregular_Punches
    {

        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
        private readonly IAttendanceHelperService _attendanceHelper;
        private readonly IOdooPushingDataService _odooPushingDataService;



        public AttendanceControllerTests_GetDashbaordData_Irregular_Punches()
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
                                        ETime = "08:30:00",
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
                                        ETime = "14:45:25",
                                        EntryExitType = 0, // Punch In
                                        DoorControllerId =  4, // Management 
                                        Access_allowed = 1
                                    },
                                    new BiometricEventDto
                                    {
                                        UserId = "163",
                                        Username = "TestUser",
                                        EDate = todayDate,
                                        ETime = "16:03:30",
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
                Calendar = employeeoOdooList?.FirstOrDefault()?.Calendar,
                Shift = filteredShifts,
                BiometricEventList = filteredBiometricEvents
            }
            };

            A.CallTo(() => _odooDataFetchingService.GetEmployeeFromOdooAsync(request)).Returns(new OdooEmployeeResponse { Data = employeeoOdooList });
            A.CallTo(() => _biometricDeviceDataFetchingService.GetBiometricEventsByDateAsync(yesterday, today)).Returns(filteredBiometricEvents);



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




            var actionResult = await _attendanceController.GetDashbaordData(163);

            // Assert: Ensure the result is an OkObjectResult
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);  // Ensure it's not null

            // Extract the value from the OkObjectResult
            var result = okResult.Value as UserActivitySummaryDto;
            Assert.NotNull(result);  // Ensure that the result list is not null


            var actual = result;


            var expected = new UserActivitySummaryDto
            {
                CheckIn = new CheckInDto
                {
                    Time = TimeSpan.Parse("08:30:00"),
                    Status = "Late",
                    Color = "red"
                },

                Activity = new List<MainUserActivity> {

                     new MainUserActivity
                     {
                         Color="red",
                         Duration=TimeSpan.Parse("07:33:30"),
                         TimeIn=TimeSpan.Parse("08:30:00"),
                         TimeOut=TimeSpan.Parse("16:03:30"),
                         Event="Main Door Entry or First Detected Punch",
                         Children=new List <SubUserActivity>
                         {

                             new SubUserActivity
                         {
                             TimeIn=TimeSpan.Parse("09:09:25"),
                             TimeOut=TimeSpan.Parse("09:30:44"),
                             Duration=TimeSpan.Parse("00:21:19"),
                             Color="green",
                             Event="EntertainmentRoom"


                         } ,

                             new SubUserActivity
                         {
                             TimeIn=TimeSpan.Parse("09:09:25"),
                             TimeOut=TimeSpan.Parse("14:45:25"),
                             Duration=TimeSpan.Parse("01:44:00"),
                             Color="green",
                             Event="EntertainmentRoom"


                         }

                         }
                     }
                 },

                CheckOutDto = new CheckoutDto
                {
                    Time = TimeSpan.Parse("16:03:30")
                }

            };







            Assert.Equal(expected.CheckIn.Time, actual.CheckIn.Time);
            Assert.Equal(expected.CheckIn.Status, actual.CheckIn.Status);
            Assert.Equal(expected.CheckIn.Color, actual.CheckIn.Color);
            Assert.Equal(expected.CheckOutDto.Time, actual.CheckOutDto.Time);
            Assert.Equal(expected.Activity.Count, actual.Activity.Count);

            Assert.Equal(expected.Activity[0].TimeIn, actual.Activity[0].TimeIn);
            Assert.Equal(expected.Activity[0].TimeOut, actual.Activity[0].TimeOut);
            Assert.Equal(expected.Activity[0].Color, actual.Activity[0].Color);
            Assert.Equal(expected.Activity[0].Duration, actual.Activity[0].Duration);
            Assert.Equal(expected.Activity[0].Children.Count, actual.Activity[0].Children.Count);
            Assert.Equal(expected.Activity[0].Event, actual.Activity[0].Event);


        }

    }
}
