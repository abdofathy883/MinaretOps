using AutoMapper;
using Core.DTOs.AttendanceBreaks;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class BreakProfile : Profile
    {
        public BreakProfile()
        {
            CreateMap<BreakPeriod, BreakDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AttendanceRecordId, opt => opt.MapFrom(src => src.AttendanceRecordId))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));
        }
    }
}
