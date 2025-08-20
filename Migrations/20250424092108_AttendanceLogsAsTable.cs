using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewAttendanceCalculationAPI.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceLogsAsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.CreateTable(
                name: "AttendanceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstPunch = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPunch = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchInDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PunchOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalEarlyTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalOverTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalSpentTimeInEntertainmentRoom = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalSpentTimeOutSideCompanyDuringWorkingHours = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalLateTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalBreakTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalActualWorkingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftDurationTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AllowedBreakTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    OverBreakTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AbsenceFlag = table.Column<bool>(type: "bit", nullable: false),
                    LateFlag = table.Column<bool>(type: "bit", nullable: false),
                    MissingPunchOutFlag = table.Column<bool>(type: "bit", nullable: false),
                    MeetingDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    AnnualLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    SickLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    CompassionateLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    AuthorizedUnpaidLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    UnauthorizedUnpaidLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    HajjLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    BusinessLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaternityLeaveDuration = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceLogs", x => x.Id);
                });

       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceLogs");

         

          
        }
    }
}
