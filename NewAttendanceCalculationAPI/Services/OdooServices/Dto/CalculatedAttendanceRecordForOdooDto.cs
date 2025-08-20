namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    public class CalculatedAttendanceRecordForOdooDto
    {
        public string EmployeeId { get; set; }
    public string UserName { get; set; }
    public DateTime FirstPunch { get; set; }
    public DateTime LastPunch { get; set; }
    public DateTime PunchOutDateTime { get; set; }
    public DateTime PunchOutDate { get; set; }
    public DateTime PunchInDateTime { get; set; }
    public DateTime PunchInDate { get; set; }
    public string PunchInTime { get; set; }
    public string PunchOutTime { get; set; }
    public string TotalEarlyTime { get; set; }
    public string TotalOverTime { get; set; }
    public string TotalSpentTimeInEntertainmentRoom { get; set; }
    public string TotalSpentTimeOutSideCompanyDuringWorkingHours { get; set; }
    public string ShiftStartTime { get; set; }
    public string ShiftEndTime { get; set; }
    public string TotalLateTime { get; set; }
    public string TotalBreakTime { get; set; }
    public string TotalActualWorkingTime { get; set; }
    public string ShiftDurationTime { get; set; }
    public string AllowedBreakTime { get; set; }
    public string OverBreakTime { get; set; }
    public bool AbsenceFlag { get; set; }
    public bool LateFlag { get; set; }
    public bool MissingPunchOutFlag { get; set; }
    public string MeetingDuration { get; set; }
    public string AnnualLeaveDuration { get; set; }
    public string SickLeaveDuration { get; set; }
    public string CompassionateLeaveDuration { get; set; }
    public string AuthorizedUnpaidLeaveDuration { get; set; }
    public string UnauthorizedUnpaidLeaveDuration { get; set; }
    public string HajjLeaveDuration { get; set; }
    public string BusinessLeaveDuration { get; set; }
    public string BreakLeaveDuration { get; set; }
    public string MaternityLeaveDuration { get; set; }
    }
}
