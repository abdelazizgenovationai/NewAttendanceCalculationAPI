﻿using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto
{
  public class CalalculatedData
    {
        public FinalEmployeeResultDto EmployeeResult { get; set; }
        public List<BiometricEventDto> UserBiometricEvents { get; set; }

        public DateTime? FirstPunch { get; set; }
        public DateTime? LastPunch { get; set; }
        public DateTime? FirstPunchIn { get; set; }
        public DateTime? LastPunchOut { get; set; }

        public DateTime ShiftStartDateTime { get; set; }
        public DateTime ShiftEndDateTime { get; set; }

        public TimeSpan? TotalSpentTimeInEntertainmentRoom { get; set; }
        public TimeSpan? TotalSpentTimeOutSideCompanyDuringWorkingHours { get; set; }

        public TimeSpan? AllowdBreakTime { get; set; }
        public TimeSpan? OverBreakTime { get; set; }
        public TimeSpan? TotalBreakTime { get; set; }
        public TimeSpan? ShiftDurationTime { get; set; }
        public TimeSpan? TotalActualWorkingTime { get; set; }
    }
}
