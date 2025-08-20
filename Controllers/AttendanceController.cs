using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace NewAttendanceCalculationAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {

        private IAttendanceCalculationService _attendanceCalculationService;
        public AttendanceController(IAttendanceCalculationService attendanceCalculationService)
        {
            _attendanceCalculationService = attendanceCalculationService;
        }


        [HttpPost("GenerateAttendanceRecords")]
        //it will return only if the user has data on both sources: Odoo and Biometric device

        public async Task<IActionResult> GenerateAttendanceRecords()
        {



            var response =  await _attendanceCalculationService.GetAttendanceLogs();
            if (response == null)
            {
                return NotFound("No holidays found.");
            }

            return Ok(response);

        }

        [HttpGet("GetDashbaordData")]
        public async Task<IActionResult> GetDashbaordData(int employeeId)
        {
            var dateFrom = DateTime.Now.AddDays(-1);
            var dateTo = DateTime.Now;


            var response = await _attendanceCalculationService.GetDashbaordData(employeeId, dateFrom, dateTo);
            if (response == null)
            {
                return NotFound("No Data found.");
            }

            return Ok(response);

        }



    }
}
