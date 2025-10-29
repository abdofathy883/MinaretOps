using AutoMapper;
using Core.DTOs.KPI;
using Core.Models;
using Infrastructure.Helpers;

namespace Infrastructure.MappingProfiles
{
    public class KPIIncedintProfile: Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;
        public KPIIncedintProfile()
        {
            CreateMap<KPIIncedint, IncedintDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.Aspect, opt => opt.MapFrom(src => src.Aspect))
                .ForMember(dest => dest.TimeStamp, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.TimeStamp, tz)))
                .ForMember(dest => dest.PenaltyPercentage, opt => opt.MapFrom(src => src.PenaltyPercentage))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.EvidenceURL, opt => opt.MapFrom(src => src.EvidenceURL))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
        }
    }
}
