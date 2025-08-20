using AutoMapper;
using NewAttendanceCalculationAPI.Models;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;

namespace AttendanceCalculationAPI.Profiles
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {

            //Source -> Target
           CreateMap<BiometricEvent, BiometricEventDto>().ReverseMap();

            

          CreateMap<AttendanceLog, AttendanceLogForOdoo>().ReverseMap();

        }



    }
}
