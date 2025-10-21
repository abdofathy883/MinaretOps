using AutoMapper;
using Core.DTOs.Attendance;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class AttendanceRecordProfile: Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        public AttendanceRecordProfile()
        {
            CreateMap<AttendanceRecord, AttendanceRecordDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.ClockIn,
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.ClockIn, tz)))
                .ForMember(dest => dest.ClockOut,
                opt => opt.MapFrom(src => src.ClockOut.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(src.ClockOut.Value, tz)
                : (DateTime?)null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.WorkDate, opt => opt.MapFrom(src => src.WorkDate))
                .ForMember(dest => dest.MissingClockOut, opt => opt.MapFrom(src => src.MissingClockOut));
        }
    }
}
