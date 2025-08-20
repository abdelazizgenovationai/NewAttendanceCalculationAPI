using Microsoft.EntityFrameworkCore;
using NewAttendanceCalculationAPI.Models;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;

namespace NewAttendanceCalculationAPI.EntityFramework
{
    public class HRSystemServiceContext:DbContext
    {
        public HRSystemServiceContext(DbContextOptions<HRSystemServiceContext> opt):base(opt){}
        public DbSet<BiometricEvent> BiometricEvents { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }



    }
}
