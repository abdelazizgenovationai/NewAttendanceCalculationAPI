﻿using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto
{
    public class IncludedEmployeeData
    {
        public List<BiometricEventDto> BiometricEvents { get; set; }
        public List<OdooEmployeeDto> Employees { get; set; }
    }
}
