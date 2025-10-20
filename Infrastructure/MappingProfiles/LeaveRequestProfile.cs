using AutoMapper;
using Core.DTOs.Attendance;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class LeaveRequestProfile: Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        public LeaveRequestProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.FromDate, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.FromDate, tz)))
                .ForMember(dest => dest.ToDate, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.ToDate, tz)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ActionDate, 
                opt => opt.MapFrom(src => src.ActionDate.HasValue 
                ? TimeZoneInfo.ConvertTimeFromUtc(src.ActionDate.Value, tz) 
                : (DateTime?)null))
                .ForMember(dest => dest.RequestDate, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.RequestDate, tz)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.ProofFile, opt => opt.MapFrom(src => src.ProofFile));
        }
    }
}
