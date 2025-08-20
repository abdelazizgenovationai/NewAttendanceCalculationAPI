using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAttendanceCalculationAPI.Helpers.Dto
{
    public sealed class CalendarType
    {
        private readonly int id;
        private readonly string name;
        private readonly string color;
        private readonly bool isDeductable;
        private readonly int numberOfDays;
        private readonly bool isWorkingDay;

        public int Id
        {
            get { return id; }
        }
        public bool IsDeductable
        {
            get { return isDeductable; }
        }
        public int NumberOfDays
        {
            get { return numberOfDays; }
        }
        public bool IsWorkingDay
        {
            get { return isWorkingDay; }
        }
        public string Name
        {
            get { return name; }
        }
        public string Color
        {
            get { return color; }
        }
        public static CalendarType Meeting { get { return new CalendarType(1, "Meeting", "#df5a55", false, 0, false); } }
        public static CalendarType AnnualLeave { get { return new CalendarType(2, "Annual Leave", "#76afcf", true, 22, true); } }
        public static CalendarType SickLeave { get { return new CalendarType(3, "Sick Leave", "#72d3b4", false, 10, true); } }
        public static CalendarType CompassionateLeave { get { return new CalendarType(4, "Compassionate Leave", "#8c9f58", false, 5, true); } }
        public static CalendarType AuthorizedUnpaidLeave { get { return new CalendarType(5, "Authorized Unpaid Leave", "#dd6495", true, 0, true); } }
        public static CalendarType UnauthorizedUnpaidLeave { get { return new CalendarType(6, "Unauthorized Unpaid Leave", "#f9bb72", true, 0, true); } }
        public static CalendarType HajjLeave { get { return new CalendarType(7, "Hajj Leave", "#43abae", false, 20, false); } }
        public static CalendarType BusinessLeave { get { return new CalendarType(8, "Business Leave", "#7D7D7D", false, 0, true); } }
        public static CalendarType BreakLeave { get { return new CalendarType(9, "Break Leave", "#7D7D7D", false, 0, true); } }
        public static CalendarType MaternityLeave { get { return new CalendarType(10, "Maternity Leave", "#CC9797", false, 35, false); } }
        private CalendarType(int id, string name, string color, bool isDeductable, int numberOfDays, bool isWorkingDay)
        {
            this.name = name;
            this.color = color;
            this.id = id;
            this.isDeductable = isDeductable;
            this.numberOfDays = numberOfDays;
            this.isWorkingDay = isWorkingDay;
        }

        public static List<CalendarType> GetAll()
        {
            List<CalendarType> list = new List<CalendarType>();
            list.Add(Meeting);
            list.Add(AnnualLeave);
            list.Add(SickLeave);
            list.Add(CompassionateLeave);
            list.Add(AuthorizedUnpaidLeave);
            list.Add(UnauthorizedUnpaidLeave);
            list.Add(MaternityLeave);
            list.Add(HajjLeave);
            list.Add(BusinessLeave);
            return list;
        }
        public static List<int> GetDeductable()
        {
            List<int> list = new List<int>();
            list.Add(Meeting.Id);
            list.Add(AnnualLeave.Id);
            list.Add(SickLeave.Id);
            list.Add(CompassionateLeave.Id);
            list.Add(MaternityLeave.Id);
            list.Add(HajjLeave.Id);
            list.Add(BusinessLeave.Id);
            return list;
        }
        public static List<CalendarType> GetAllButMaternity()
        {
            List<CalendarType> list = new List<CalendarType>();
            list.Add(Meeting);
            list.Add(AnnualLeave);
            list.Add(SickLeave);
            list.Add(CompassionateLeave);
            list.Add(AuthorizedUnpaidLeave);
            list.Add(UnauthorizedUnpaidLeave);
            list.Add(HajjLeave);
            list.Add(BusinessLeave);
            return list;
        }
    }
}
