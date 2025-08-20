using AutoMapper;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Controllers;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace AttendanceCalculationAPI.AttendanceCalculationAPI.Tests
{
    public class AttendanceControllerTests
    {
        private readonly AttendanceController _attendanceController;
        private readonly IAttendanceCalculationService _attendanceCalculationService;
        private readonly IBiometricDeviceDataFetchingService _biometricDeviceDataFetchingService;
        private readonly IOdooDataFetchingService _odooDataFetchingService;
        private readonly IOdooPushingDataService _odooPushingDataService;
        private readonly IAttendanceHelperService _attendanceHelper;
        private readonly IConfiguration _configuration;

        public AttendanceControllerTests()
        {


            var fakeMapper = A.Fake<IMapper>();



            _biometricDeviceDataFetchingService = A.Fake<IBiometricDeviceDataFetchingService>();
            _odooDataFetchingService = A.Fake<IOdooDataFetchingService>();
            _odooPushingDataService= A.Fake<IOdooPushingDataService>();
            _attendanceHelper = A.Fake<IAttendanceHelperService>();
            _configuration = A.Fake<IConfiguration>();
            var options = new DbContextOptionsBuilder<HRSystemServiceContext>()
    .UseInMemoryDatabase(databaseName: "TestDB")
    .Options;

            var fakeContext = new HRSystemServiceContext(options);

            _attendanceCalculationService = new AttendanceCalculationService(fakeContext,fakeMapper,_odooPushingDataService, _attendanceHelper, _configuration);


            _attendanceController = new AttendanceController(_attendanceCalculationService);

        }

        [Fact]
        public void AttendanceController_GenerateAttendanceRecords_ReturnsList()
        {


            // Arrange 


            var calendarFaker = new Faker<CalendarDto>()
    .RuleFor(c => c.DateFrom, f => f.Date.Past().ToString("yyyy-MM-dd"))
    .RuleFor(c => c.DateTo, f => f.Date.Future().ToString("yyyy-MM-dd"))
    .RuleFor(c => c.Duration, f => f.Random.Double(1, 8).ToString("0.0"))
    .RuleFor(c => c.TypeId, f => f.Random.Guid().ToString())
    .RuleFor(c => c.TypeName, f => f.PickRandom("Leave", "Work", "Remote"));

            var shiftFaker = new Faker<ShiftDto>()
                .RuleFor(s => s.StartDateTime, f => f.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss"))
                .RuleFor(s => s.EndDateTime, f => f.Date.Soon().ToString("yyyy-MM-ddTHH:mm:ss"))
                .RuleFor(s => s.Name, f => f.Name.JobTitle())
                .RuleFor(s => s.Id, f => f.IndexFaker + 1);

            var employeeFaker = new Faker<OdooEmployeeDto>()
                .RuleFor(e => e.EmployeeId, f => f.IndexFaker + 1000)
                .RuleFor(e => e.EmployeeUniqueNumber, f => f.Random.AlphaNumeric(8))
                .RuleFor(e => e.Calendar, f => calendarFaker.Generate(2))
                .RuleFor(e => e.Shift, f => shiftFaker.Generate(1));

            var biometricEventFaker = new Faker<BiometricEventDto>()
                .RuleFor(e => e.UserId, f => f.Random.Guid().ToString())
                .RuleFor(e => e.Username, f => f.Person.UserName)
                .RuleFor(e => e.EDate, f => f.Date.Past().ToString("yyyy-MM-dd"))
                .RuleFor(e => e.ETime, f => f.Date.Recent().ToString("HH:mm:ss"))
                .RuleFor(e => e.EntryExitType, f => f.PickRandom(new[] { 0, 1 }))
                .RuleFor(e => e.Access_allowed, f => f.PickRandom(new[] { 0, 1 }))
               .RuleFor(e => e.DoorControllerId, f => (int)f.PickRandom(Enum.GetValues<AccessControlDoor>()));

            //var holidays = await _odppService.GetHolidayAsync(new GetHolidayRequest { DateFrom=null,DateTo=null});
            var yesterday = DateTime.Now.AddDays(-1);
            var today = DateTime.Now;

            var request = new OdooEmployeeRequest
            {
                DateFrom = yesterday.ToString("yyyy-MM-dd"),
                DateTo = today.ToString("yyyy-MM-dd"),
                EmployeeId = null
            };
            var employeeListFromOdoo = employeeFaker.Generate(200);
            var employeesBiometricEvents = biometricEventFaker.Generate(1000);


            A.CallTo(() => _odooDataFetchingService.GetEmployeeFromOdooAsync(request)).Returns(new OdooEmployeeResponse { Data = employeeListFromOdoo });
            A.CallTo(() => _biometricDeviceDataFetchingService.GetBiometricEventsByDateAsync(yesterday, today)).Returns(employeesBiometricEvents);
            //  A.CallTo(() => _attendanceHelper.GetFullAttendanceInfo(employeeListFromOdoo, employeesBiometricEvents)).Returns(emlpoyeeFullRecords);



            var result = _attendanceController.GenerateAttendanceRecords();

            result.Should().BeOfType<Task<IActionResult>>();


        }
    }
}
