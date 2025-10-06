using AutoMapper;
using Core.DTOs.Attendance;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class AttendanceRecordProfile: Profile
    {
        public AttendanceRecordProfile()
        {
            CreateMap<AttendanceRecord, AttendanceRecordDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.ClockIn, opt => opt.MapFrom(src => src.ClockIn))
                .ForMember(dest => dest.ClockOut, opt => opt.MapFrom(src => src.ClockOut))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress));
        }
    }
}
