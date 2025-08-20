using Microsoft.AspNetCore.Mvc;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices;
using System.Globalization;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Helpers;
using Serilog.Context;


namespace NewAttendanceCalculationAPI.Controllers
{
    //api/commands
    //[controller ] means the name before Controller word in class name
    [Route("api/[controller]")]
    [ApiController]
    public class OdooFetchingDataController:ControllerBase
    {

        private readonly HelperService _helperService;
        private readonly OdooDataFetchingService _odooDataFetchingService;
        private readonly BiometricDeviceDataFetchingService _biometricService;
        private readonly ILogger<OdooFetchingDataController> _logger;


        public OdooFetchingDataController(HelperService helperService, OdooDataFetchingService odooDataFetchingService, BiometricDeviceDataFetchingService biometricService, ILogger<OdooFetchingDataController> logger)
        {
            _helperService = helperService;
            _odooDataFetchingService = odooDataFetchingService;
            _biometricService = biometricService;
            _logger = logger;
        }


        [HttpPost("GetHoliday")]
        public async Task<IActionResult> GetHoliday(string? DateFrom, string? DateTo, string? Name)
        {
            using (LogContext.PushProperty("Endpoint", nameof(GetHoliday)))
            using (LogContext.PushProperty("DateFrom", DateFrom))
            using (LogContext.PushProperty("DateTo", DateTo))
            using (LogContext.PushProperty("Name", Name))
            {
                try
                {
                    _logger.LogInformation("Getting sample data");



                    var request = new GetHolidayRequest
                    {
                        DateFrom = DateFrom,
                        DateTo = DateTo,
                        Name = Name
                    };

                    _logger.LogDebug("GetHoliday request created: {@Request}", request);

                    var response = await _odooDataFetchingService.GetHolidayAsync(request);
                    if (response == null)
                    {
                        _logger.LogWarning("No holidays found for the given criteria");
                        return NotFound("No holidays found.");
                    }

                    _logger.LogInformation("Successfully retrieved {HolidayCount} holidays", response.Data.Count);

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing GetHoliday request");
                    return StatusCode(500, "An error occurred while processing your request.");
                } 
            }

        }


        [HttpPost("GetCalendar")]
        public async Task<IActionResult> GetCalendar(string? DateFrom, string? DateTo, int? EmployeeId)
        {
            using (LogContext.PushProperty("Endpoint", nameof(GetCalendar)))
            using (LogContext.PushProperty("DateFrom", DateFrom))
            using (LogContext.PushProperty("DateTo", DateTo))
            using (LogContext.PushProperty("EmployeeId", EmployeeId))
            {
                try
                {
                    _logger.LogInformation("Starting GetCalendar request processing");

                    var request = new GetCalendarRequest
                    {
                        DateFrom = DateFrom,
                        DateTo = DateTo,
                        EmployeeId = EmployeeId
                    };

                    _logger.LogDebug("GetCalendar request created: {@Request}", request);

                    var response = await _odooDataFetchingService.GetCalendarAsync(request);
                    if (response == null)
                    {
                        _logger.LogWarning("No calendars found for the given criteria");
                        return NotFound("No Calendars found.");
                    }

                    _logger.LogInformation("Successfully retrieved calendar data");
                    return Ok(response);
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error processing GetCalendar request");
                    return StatusCode(500, "An error occurred while processing your request.");
                } 
            }

        }

        [HttpPost("GetAttendance")]
        public async Task<IActionResult> GetAttendance(string? DateFrom, string? DateTo, int? EmployeeId)
        {
            using (LogContext.PushProperty("Endpoint", nameof(GetAttendance)))
            using (LogContext.PushProperty("DateFrom", DateFrom))
            using (LogContext.PushProperty("DateTo", DateTo))
            using (LogContext.PushProperty("EmployeeId", EmployeeId))
            {
                try
                {
                    _logger.LogInformation("Starting GetAttendance request processing");

                    var request = new GetAttendanceRequest
                    {
                        DateFrom = DateFrom,
                        DateTo = DateTo,
                        EmployeeId = EmployeeId
                    };

                    _logger.LogDebug("GetAttendance request created: {@Request}", request);

                    var response = await _odooDataFetchingService.GetAttendanceAsync(request);
                    if (response == null)
                    {
                        _logger.LogWarning("No attendance records found for the given criteria");
                        return NotFound("No attendance found.");
                    }

                    _logger.LogInformation("Successfully retrieved {AttendanceCount} attendance records", response.Data.Count());

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing GetAttendance request");
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

        }

        //[HttpGet("events/{date}")]
        //[HttpGet("biometricDevice/attendance/{date}")]
        [HttpGet("biometricDevice/attendance/{date}")]
        public async Task<ActionResult<List<BiometricEventDto>>> GetEvents([FromRoute] string fromDate,string toDate)
        {
            using (LogContext.PushProperty("Endpoint", nameof(GetEvents)))
            using (LogContext.PushProperty("FromDate", fromDate))
            using (LogContext.PushProperty("ToDate", toDate))
            {
                try
                {
                    _logger.LogInformation("Starting GetEvents request processing");

                    if (!DateTime.TryParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue1)
                        ||
                        !DateTime.TryParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue2))
                    {
                        _logger.LogWarning("Invalid date format received. Expected format: dd-MM-yyyy");
                        return BadRequest("Invalid date format. Please use dd-MM-yyyy format (e.g., 01-10-2024).");
                    }

                    var events = await _biometricService.GetBiometricEventsByDateAsync(DateTime.Parse(fromDate), DateTime.Parse(toDate));
                    _logger.LogInformation("Successfully retrieved {EventCount} biometric events", events.Count);
                    return Ok(events);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching biometric events");
                    return StatusCode(500, "An error occurred while processing your request.");
                } 
            }
        }

        /// <summary>
        /// Retrieves employee shifts and calendar events for a specific date range from Odoo
        /// </summary>
        /// <param name="employeeId">Optional employee ID to filter results</param>
        /// <param name="dateFrom">Start date in YYYY-MM-DD format</param>
        /// <param name="dateTo">End date in YYYY-MM-DD format</param>
        /// <returns>List of employee shifts and calendar events from Odoo</returns>
        /// <response code="200">Returns employee shifts data from Odoo</response>
        /// <response code="404">No shifts found for criteria</response>
        [HttpGet("GetEmployeeShifts")] 
        [ProducesResponseType(typeof(OdooEmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEmployeeShifts(int? employeeId, string? dateFrom, string? dateTo)
        {
            using (LogContext.PushProperty("Endpoint", nameof(GetEmployeeShifts)))
            using (LogContext.PushProperty("EmployeeId", employeeId))
            using (LogContext.PushProperty("DateFrom", dateFrom))
            using (LogContext.PushProperty("DateTo", dateTo))
            {
                try
                {
                    _logger.LogInformation("Starting GetEmployeeShifts request processing");

                    var request = new OdooEmployeeRequest
                    {
                        EmployeeId = employeeId,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    };

                    _logger.LogDebug("GetEmployeeShifts request created: {@Request}", request);

                    var response = await _odooDataFetchingService.GetEmployeeFromOdooAsync(request);

                    if (response == null || !response.Data.Any())
                    {
                        _logger.LogWarning("No shift data found for the specified criteria");
                        return NotFound("No shift data found for the specified criteria.");
                    }

                    _logger.LogInformation("Successfully retrieved {ShiftCount} employee shifts", response.Data.Count());
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing GetEmployeeShifts request");
                    return StatusCode(500, "An error occurred while processing your request.");
                } 
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportData()
        {
            using (LogContext.PushProperty("Endpoint", nameof(ImportData)))
            {
                try
                {
                    _logger.LogInformation("Starting data import process");

                    await _helperService.ReadAndInsertBiometricEventsAsync();

                    _logger.LogInformation("Data imported successfully");
                    return Ok("Data imported successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during data import");
                    return StatusCode(500, "An error occurred while importing data.");
                }
            }
        }





    }

}
