using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using System.Globalization;

namespace NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions
{
    public static class AttendanceExtensions
    {
        public static bool OnlyShiftsThatStartTodayAndEndToday(this List<ShiftDto> shifts)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var today = DateTime.Now.Date;
            var result = shifts.Any(shift =>
            DateTime.Parse(shift.StartDateTime).Date == today &&
            DateTime.Parse(shift.EndDateTime).Date == today);

            return result;

        }


        public static bool OnlyShiftsThatStartYesterdayAndEndToday(this List<ShiftDto> shifts)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var today = DateTime.Now.Date;
            var result = shifts.Any(shift => DateTime.Parse(shift.StartDateTime).Date == yesterday.Date && DateTime.Parse(shift.EndDateTime).Date == today.Date);

            return result;

        }



        public static TimeSpan GetNonNegativeTimeSpan(this TimeSpan input)
        {
            try
            {
                var result = input < TimeSpan.Zero ? TimeSpan.Zero : input;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while applying GetNonNegativeTimeSpan", ex);
            }
        }

        public static DateTime ConvertStringToDateTime(string date, string time)
        {
            try
            {
                var result = DateTime.ParseExact($"{date} {time}", "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-GB"));
                return result;
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Invalid date/time format: '{date} {time}'. Expected format: 'dd/MM/yyyy HH:mm:ss'", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while parsing date/time '{date} {time}'", ex);
            }
        }



        /// <summary>
        /// Returns a new TimeSpan with milliseconds set to zero.
        /// </summary>
        public static TimeSpan StripMilliseconds(this TimeSpan? timeSpan)
        {
            return new TimeSpan(
                timeSpan.Value. Hours,
                timeSpan.Value.Minutes,
                timeSpan.Value.Seconds
            );
        }

    }
}
