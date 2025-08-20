using System.Reflection;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto
{
    public class AttendanceLogForOdoo
    {

            public string EmployeeId { get; set; }
    public string UserName { get; set; }

    public DateTime? FirstPunch { get; set; }
    public DateTime? LastPunch { get; set; }

    public DateTime? PunchOutDateTime { get; set; }
    public DateTime? PunchOutDate { get; set; }
    public DateTime? PunchInDateTime { get; set; }
    public DateTime? PunchInDate { get; set; }

    public TimeSpan? PunchInTime { get; set; }
    public TimeSpan? PunchOutTime { get; set; }
    public TimeSpan? TotalEarlyTime { get; set; }
    public TimeSpan? TotalOverTime { get; set; }
    public TimeSpan? TotalSpentTimeInEntertainmentRoom { get; set; }
    public TimeSpan? TotalSpentTimeOutSideCompanyDuringWorkingHours { get; set; }

    public TimeSpan? ShiftStartTime { get; set; }
    public TimeSpan? ShiftEndTime { get; set; }

    public TimeSpan? TotalLateTime { get; set; }
    public TimeSpan? TotalBreakTime { get; set; }
    public TimeSpan? TotalActualWorkingTime { get; set; }
    public TimeSpan? ShiftDurationTime { get; set; }
    public TimeSpan? AllowedBreakTime { get; set; }
    public TimeSpan? OverBreakTime { get; set; }

    public bool AbsenceFlag { get; set; }
    public bool LateFlag { get; set; }
    public bool MissingPunchOutFlag { get; set; }

    public TimeSpan? MeetingDuration { get; set; }
    public TimeSpan? AnnualLeaveDuration { get; set; }
    public TimeSpan? SickLeaveDuration { get; set; }
    public TimeSpan? CompassionateLeaveDuration { get; set; }
    public TimeSpan? AuthorizedUnpaidLeaveDuration { get; set; }
    public TimeSpan? UnauthorizedUnpaidLeaveDuration { get; set; }
    public TimeSpan? HajjLeaveDuration { get; set; }
    public TimeSpan? BusinessLeaveDuration { get; set; }
    public TimeSpan? BreakLeaveDuration { get; set; }
    public TimeSpan? MaternityLeaveDuration { get; set; }



     
    }

}
