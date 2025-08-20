using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using System.Globalization;

namespace NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Extensions
{
    using NewAttendanceCalculationAPI.Helpers.Dto;
    using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
    using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class AttendanceLogStatisticsExtensions
    {

        const int punchInAction = (int)EmployeeAction.PunchIn;
        const int punchOutAction = (int)EmployeeAction.PunchOut;

        private static readonly List<int> listOfMainDoors = new List<int>
{
    (int)AccessControlDoor.ReceptionMain,
    (int)AccessControlDoor.Attendance,
   (int) AccessControlDoor.InformationTechnology,
    (int)AccessControlDoor.ElectronicPayments


};



        public static DateTime FirstPunchIn(this List<BiometricEventDto> userBiometricEvents)
        {

            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }
                

                DateTime? firstPunchIn = userBiometricEvents.Where(x => x.EntryExitType == punchInAction && listOfMainDoors.Contains(x.DoorControllerId))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime))
                    .FirstOrDefault();


                if (firstPunchIn == DateTime.MinValue)
                {
                    return userBiometricEvents.FirstPunch();
                }

                return (DateTime)firstPunchIn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating FirstPunchIn", ex);
            }
        }

        public static DateTime FirstPunch(this List<BiometricEventDto> userBiometricEvents)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }

                var result = userBiometricEvents
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime))
                    .FirstOrDefault();


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating FirstPunchIn", ex);
            }
        }

        public static DateTime LastPunchIn(this List<BiometricEventDto> userBiometricEvents)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                DateTime? lastPunchOut = userBiometricEvents
                    .Where(x => x.EntryExitType == punchInAction && listOfMainDoors.Contains(x.DoorControllerId))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime)).LastOrDefault();

                if (lastPunchOut == null)
                {
                    return userBiometricEvents.LastPunch();
                }


                return (DateTime)lastPunchOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }

        public static DateTime LastPunchOut(this List<BiometricEventDto> userBiometricEvents)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                DateTime? lastPunchOut = userBiometricEvents
                    .Where(x => x.EntryExitType == punchOutAction && listOfMainDoors.Contains(x.DoorControllerId))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime)).LastOrDefault();

                if (lastPunchOut.Equals(DateTime.MinValue))
                {
                    return userBiometricEvents.LastPunch();
                }


                return (DateTime)lastPunchOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }
        public static DateTime LastPunch(this List<BiometricEventDto> userBiometricEvents)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                var result = userBiometricEvents

                    .OrderBy(x => x.ETime)
                    .Select(x =>
                    {

                        //var test1 = x.EDate;
                        //var test2 = x.ETime;

                        return AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime);
                    }




                    ).LastOrDefault();


               

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }



        public static DateTime FirstPunchInAfterConfirmedShiftInterval(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {

            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }



                DateTime? firstPunchIn = userBiometricEvents.Where(x => x.EntryExitType == punchInAction && x.OnlyWithinOverTimeInterval(shiftStartDateTime, shiftEndDateTime))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime))
                    .FirstOrDefault();


                if (firstPunchIn == null)
                {
                    return userBiometricEvents.FirstPunchAfterConfirmedShiftInterval(shiftStartDateTime, shiftEndDateTime);
                }

                return (DateTime)firstPunchIn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating FirstPunchIn", ex);
            }
        }

        public static DateTime FirstPunchAfterConfirmedShiftInterval(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }

                var result = userBiometricEvents.Where(x => x.OnlyWithinOverTimeInterval(shiftStartDateTime, shiftEndDateTime))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime))
                    .FirstOrDefault();


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating FirstPunchIn", ex);
            }
        }

        public static DateTime LastPunchInAfterConfirmedShiftInterval(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                DateTime? lastPunchOut = userBiometricEvents
                    .Where(x => x.EntryExitType == punchInAction && x.OnlyWithinOverTimeInterval(shiftStartDateTime, shiftEndDateTime))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime)).LastOrDefault();

                if (lastPunchOut == null)
                {
                    return userBiometricEvents.LastPunchAfterConfirmedShiftInterval(shiftStartDateTime, shiftEndDateTime);
                }


                return (DateTime)lastPunchOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }

        public static DateTime LastPunchOutAfterConfirmedShiftInterval(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                DateTime? lastPunchOut = userBiometricEvents
                    .Where(x => x.EntryExitType == punchOutAction && x.OnlyWithinOverTimeInterval(shiftStartDateTime, shiftEndDateTime))
                    .OrderBy(x => x.ETime)
                    .Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime)).LastOrDefault();

                if (lastPunchOut == null)
                {
                    return userBiometricEvents.LastPunchAfterConfirmedShiftInterval(shiftStartDateTime, shiftEndDateTime);
                }


                return (DateTime)lastPunchOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }
        public static DateTime LastPunchAfterConfirmedShiftInterval(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                {
                    return DateTime.MinValue;
                }


                // var test=
                var result = userBiometricEvents
                    .Where(x => x.OnlyWithinOverTimeInterval(shiftStartDateTime, shiftEndDateTime))
                    .OrderBy(x => x.ETime)
                    .Select(x =>
                    {

                        //var test1 = x.EDate;
                        //var test2 = x.ETime;

                        return AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime);
                    }




                    ).LastOrDefault();




                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating LastPunchOut", ex);
            }
        }




        public static TimeSpan TotalEarlyTime(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime)
        {
            try
            {
                var punchIn = userBiometricEvents.FirstPunchIn();
                var calculationResult = shiftStartDateTime - punchIn;
                var result = calculationResult.GetNonNegativeTimeSpan();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating TotalEarlyTime", ex);
            }
        }

        public static TimeSpan TotalOverTime(this List<BiometricEventDto> userBiometricEvents, DateTime shiftEndDateTime, DateTime shiftStartDateTime)
        {
            try
            {
                var lastPunchOut = userBiometricEvents.LastPunchOut();
                var lastPunchIn= userBiometricEvents.LastPunchIn();


                var lastPunch = userBiometricEvents.LastPunch();

                var start = DateTime.MaxValue;
                var end = DateTime.MaxValue;
                var result = TimeSpan.Zero;



              

                if(lastPunchOut > lastPunchIn && lastPunchOut > shiftEndDateTime) // keep working after end shift time without leaving company
                {

                    end = lastPunchOut;
                    start = shiftEndDateTime;
                    
                }else if (lastPunchIn > lastPunchOut && userBiometricEvents.MissingPunchOut() ) //he left then come back after some time and keep working until next day
                {
                    end = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    start = userBiometricEvents.FirstPunchAfterConfirmedShiftInterval(shiftEndDateTime, shiftStartDateTime);



                }
                else if (lastPunchIn > shiftEndDateTime && !userBiometricEvents.MissingPunchOut()) //he left then come back after some time and left before the end of day
                {
                    end = lastPunchOut;
                    start = userBiometricEvents.FirstPunchAfterConfirmedShiftInterval(shiftEndDateTime, shiftStartDateTime);


                }
               


                result = end - start;

                var finalResult = result.GetNonNegativeTimeSpan();
                return finalResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating TotalOverTime", ex);
            }
        }

        public static TimeSpan TotalLateTime(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime,TimeSpan acceptedLateTime)
        {
            try
            {
                var punchIn = userBiometricEvents.FirstPunchIn();
                var calculationResult = (punchIn - acceptedLateTime) - shiftStartDateTime;
                var result = calculationResult.GetNonNegativeTimeSpan();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating TotalLateTime", ex);
            }
        }

        public static bool LateFlag(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, TimeSpan acceptedLateTime)
        {

            try
            {
                var result = userBiometricEvents.TotalLateTime(shiftStartDateTime, acceptedLateTime) > TimeSpan.Zero;
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while LateFlag", ex);
            }
        }

        public static TimeSpan TotalSpentTimeInSpecifiedRoom(this List<BiometricEventDto> userBiometricEvents, List<int> roomId)
        {

            try
            {
                // Step 1: Filter only valid access events for the entertainment room, ordered chronologically
                var eventsList = userBiometricEvents
                .Where(e => roomId.Contains(e.DoorControllerId) && e.Access_allowed == 1)
                //.OrderBy(e => DateTime.Parse($"{e.EDate} {e.ETime}"))
                .OrderBy(e => DateTime.ParseExact($"{e.EDate} {e.ETime}", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))

                .ToList();

                TimeSpan total = TimeSpan.Zero;
                DateTime? entry = null;

                // Step 2: Iterate through events
                foreach (var eventItem in eventsList)
                {
                    var timestamp = DateTime.ParseExact($"{eventItem.EDate} {eventItem.ETime}", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    // below will hold the punch in time 
                    entry = eventItem.EntryExitType == punchInAction && entry == null ? timestamp : entry;

                    if (eventItem.EntryExitType == punchOutAction && entry != null)
                    {
                        // here timestamp will represnt the punch out time
                        total += timestamp - entry.Value;
                        entry = null;
                    }
                }

                return total;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while TotalSpentTimeInSpecifiedRoom", ex);
            }
        }

        public static TimeSpan TotalTimeSpentOutsideCompany(this List<BiometricEventDto> events, DateTime? shiftStart, DateTime? shiftEnd)
        {


            try
            {


                var endTime = shiftEnd;
                var lastPunchOut = events.LastPunchOut();


                if (shiftEnd < events.LastPunchIn())
                {

                    if (!lastPunchOut.Equals(DateTime.MinValue) && lastPunchOut > events.LastPunchIn())
                    {
                        endTime = lastPunchOut;
                    }
                    else
                    {
                        endTime = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }

                }
               





               const int punchIn = 0;
                const int punchOut = 1;

                // Filter events within shift range, valid, and only relevant doors
                var filteredEvents = events
                    .Where(e => listOfMainDoors.Contains(e.DoorControllerId)
                                && e.Access_allowed == 1)
                    .Select(e => new
                    {
                        e.EntryExitType,
                        Timestamp = AttendanceExtensions.ConvertStringToDateTime(e.EDate, e.ETime)
                    })
                    .Where(e => e.Timestamp >= shiftStart && e.Timestamp <= endTime)
                    .OrderBy(e => e.Timestamp)
                    .ToList();

                TimeSpan totalOutsideTime = TimeSpan.Zero;
                DateTime? exitTime = null;

                foreach (var ev in filteredEvents)
                {
                    // EntryExitType == 1 → Punch Out → leaving the company
                    if (ev.EntryExitType == 1 && exitTime == null)
                    {
                        exitTime = ev.Timestamp; // Start counting time outside
                    }
                    // EntryExitType == 0 → Punch In → back to company
                    else if (ev.EntryExitType == 0 && exitTime != null)
                    {
                        totalOutsideTime += ev.Timestamp - exitTime.Value;
                        exitTime = null;
                    }
                }


                // If still outside at end of shift, count till shiftEnd
                if (exitTime != null)
                {
                    totalOutsideTime += (TimeSpan)(shiftEnd - exitTime.Value);
                }

                return totalOutsideTime;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating TotalTimeSpentOutsideCompany", ex);
            }
        }
        public static bool AbsenceFlag(this List<BiometricEventDto> userBiometricEvents)
        {

            return userBiometricEvents.Count == 0;

        }

        public static bool AbsenceFlag(this List<ShiftDto> employeeShifts)
        {

            return employeeShifts.Count == 0;



        }

        public static bool MissingPunchOut(this List<BiometricEventDto> userBiometricEvents)
        {
            try
            {
                if (userBiometricEvents == null || !userBiometricEvents.Any())
                    return true;


                // var test=
                var lastPunch = userBiometricEvents

                    .OrderBy(x => x.ETime).LastOrDefault();

                // check if punch out is missing from any of the main doors 
                var result = lastPunch?.EntryExitType != punchOutAction && !listOfMainDoors.Contains(lastPunch.DoorControllerId);


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating MissingPunchOut", ex);
            }
        }

        public static TimeSpan CalculateDurationForType(this List<CalendarDto> calednarList, string calendarName, DateTime shiftStart, DateTime shiftEnd)
        {

            try
            {

                if (calednarList.Count <= 0)
                {
                    return TimeSpan.Zero;
                }




                var totalShiftTicks = (shiftEnd - shiftStart).Ticks;


                var totalCalendarTicks = calednarList
                               .Where(x => x.TypeName.Equals(calendarName))
                               .Select(y =>
                               /*in case the end of leave was in the mid of the day or before the shift end date*/
                              shiftEnd > DateTime.Parse(y.DateTo) ? (DateTime.Parse(y.DateTo) - shiftStart).Ticks :

                               /*in case the */
                               (DateTime.Parse(y.DateTo) - DateTime.Parse(y.DateFrom)).Ticks

                               )
                               .Sum();
                // Fetching the accurate load for a single day, since the calendar interval represents the total load, not the daily value.
                var result = totalCalendarTicks > totalShiftTicks ? totalShiftTicks : totalCalendarTicks;


                return TimeSpan.FromTicks(result);
            }
            catch (Exception)
            {

                return TimeSpan.Zero;
            }


        }



        public static TimeSpan CalculateDurationForMeetings(this List<CalendarDto> calednarList, List<BiometricEventDto> events, DateTime shiftStart, DateTime shiftEnd)
        {

            try
            {

                if (calednarList.Count <= 0)
                {
                    return TimeSpan.Zero;
                }




                var totalShiftTicks = (shiftEnd - shiftStart).Ticks;


                var totalCalendarTicks = calednarList
                               .Select(meeting =>

                               {
                                   var meetingStart = DateTime.Parse(meeting.DateFrom);
                                   var meetingEnd = DateTime.Parse(meeting.DateTo);

                                   DateTime? detectedPunchDuringCalendarHours= events?.Where(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) > meetingStart && AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) < meetingEnd).Select(x => AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime)).FirstOrDefault();

//#if DEBUG

//                                   var intersectedHours = new List<BiometricEventDto>();
//                                   foreach(var x in events){

//                                       if(AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) > meetingStart && AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) < meetingEnd).Select(x => DateTime.Parse(x.EDate))
//                                       {
//                                           intersectedHours.Add(x);
//                                       }


//                                                }

//#endif



                                   if(!detectedPunchDuringCalendarHours.Equals(DateTime.MinValue))
                                   {
                                       meetingEnd = (DateTime)detectedPunchDuringCalendarHours;
                                   }

                               var result=(meetingEnd - meetingStart).Ticks;

                               return result;
                               }
                               )
                               .Sum();
                // Fetching the accurate load for a single day, since the calendar interval represents the total load, not the daily value.
                var result = totalCalendarTicks > totalShiftTicks ? totalShiftTicks : totalCalendarTicks;


                return TimeSpan.FromTicks(result);
            }
            catch (Exception)
            {

                return TimeSpan.Zero;
            }


        }


        public static DateTime ShiftStartDateTime(this List<ShiftDto> shiftList)
        {
            try
            {
                string shiftStart = shiftList.FirstOrDefault()?.StartDateTime;
                

                var result= !string.IsNullOrEmpty(shiftStart) ? DateTime.Parse(shiftStart) : DateTime.MinValue;

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception($"ShiftStartDateTime issue:{ex.Message} + Inner Exception:{ex.InnerException?.Message}");
            }

        }

        public static DateTime ShiftEndDateTime(this List<ShiftDto> shiftList)
        {
            try
            {
                string shiftEnd = shiftList.FirstOrDefault()?.EndDateTime;

                var result = !string.IsNullOrEmpty(shiftEnd) ? DateTime.Parse(shiftEnd) : DateTime.MinValue;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"error while fetching ShiftEndDateTime:{ex.Message} + Inner Exception:{ex.InnerException?.Message}");
            }

        }


        public static TimeSpan TotalBreakTime( this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime,DateTime shiftEndDateTime)
        {
            try
            {

                var totalSpentTimeInEntertainmentRoom = userBiometricEvents?.TotalSpentTimeInSpecifiedRoom(new List<int> { (int)AccessControlDoor.EntertainmentRoom });
                var totalSpentTimeOutSideCompanyDuringWorkingHours = userBiometricEvents?.TotalTimeSpentOutsideCompany(shiftStartDateTime, shiftEndDateTime);


                var totalBreakTime = (totalSpentTimeOutSideCompanyDuringWorkingHours ?? TimeSpan.Zero)+ (totalSpentTimeInEntertainmentRoom ?? TimeSpan.Zero);


                return totalBreakTime;

            }
            catch(Exception ex)
            {
                throw new Exception($"error while calculating TotalBreakTime:{ex.Message} + Inner Exception:{ex.InnerException?.Message}");

            }
        }


        public static TimeSpan AllowdBreakTime()
        {
            var result = TimeSpan.FromHours(1);
            return result;
        }

        public static TimeSpan OverBreakTime(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {

            try
            {

             TimeSpan allowdBreakTime = AllowdBreakTime();

             var totalBreakTime = userBiometricEvents.TotalBreakTime(shiftStartDateTime, shiftEndDateTime);

            var overBreakTime = allowdBreakTime > totalBreakTime ? TimeSpan.Zero : totalBreakTime - allowdBreakTime;

            return overBreakTime;

            }
            catch (Exception ex)
            {
                throw new Exception($"error while calculating OverBreakTime:{ex.Message} + Inner Exception:{ex.InnerException?.Message}");

            }
        }


        public static TimeSpan TotalActualWorkingTime(this List<BiometricEventDto> userBiometricEvents, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            try
            {

                var lastPunchOut = userBiometricEvents.LastPunchOut();
                var lastPunchIn = userBiometricEvents.LastPunchIn();


                // than means employee came after working hours and his shift extended to next day, so for today we need to close that by end of the same day.

                if (lastPunchIn > shiftEndDateTime && lastPunchOut < lastPunchIn ) { 
                    lastPunchOut= DateTime.Today.AddDays(1).AddMilliseconds(-1);
                }

                

                var result = (lastPunchOut - userBiometricEvents.FirstPunchIn()) - userBiometricEvents.TotalBreakTime(shiftStartDateTime, shiftEndDateTime);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"error while calculating OverBreakTime:{ex.Message} + Inner Exception:{ex.InnerException?.Message}");

            }
        }
    
        public static bool OnlyWithinOverTimeInterval(this BiometricEventDto x, DateTime shiftStartDateTime, DateTime shiftEndDateTime)
        {
            var startDateTime = DateTime.Today;
            var endDateTime = DateTime.Now.AddDays(1).AddMilliseconds(-1);

            var result = (AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) < shiftStartDateTime && AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) > startDateTime) ||

                (AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) > shiftEndDateTime && AttendanceExtensions.ConvertStringToDateTime(x.EDate, x.ETime) < endDateTime);


            return result;
        }
    
    }

}
