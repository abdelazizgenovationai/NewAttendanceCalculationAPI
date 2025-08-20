using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace NewAttendanceCalculationAPI.Services.AttendanceServices.Dto
{
    public class AttendanceLog
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }

        public int EmployeeId { get; set; }
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

        public TimeSpan? TotalTimeWorkedWithMeetings { get; set; }

        public TimeSpan? ShiftDurationTime { get; set; }
        public TimeSpan? AllowedBreakTime { get; set; }

        public TimeSpan? OverBreakTime { get; set; }

        public bool AbsenceFlag { get; set; } = false;
        public bool LateFlag { get; set; } = false;


        public bool MissingPunchOutFlag { get; set; } = false;



        public TimeSpan? MeetingDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? AnnualLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? SickLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? CompassionateLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? AuthorizedUnpaidLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? UnauthorizedUnpaidLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? HajjLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? BusinessLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? BreakLeaveDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan? MaternityLeaveDuration { get; set; } = TimeSpan.Zero;

        public DateTime CreatedDate { get; set; }

        //  Central method to reset negative TimeSpans
        public void ResetNegativeTimeSpans()
        {
            // Get all TimeSpan? properties using reflection
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(TimeSpan?) && prop.CanWrite)
                {
                    var value = (TimeSpan?)prop.GetValue(this);
                    if (value.HasValue && value.Value.Ticks < 0)
                    {
                        prop.SetValue(this, TimeSpan.Zero);
                    }
                }
            }
        }

    }
}
