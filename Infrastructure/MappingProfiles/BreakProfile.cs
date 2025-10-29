using AutoMapper;
using Core.DTOs.AttendanceBreaks;
using Core.Models;
using Infrastructure.Helpers;

namespace Infrastructure.MappingProfiles
{
    public class BreakProfile : Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;
        public BreakProfile()
        {
            CreateMap<BreakPeriod, BreakDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AttendanceRecordId, opt => opt.MapFrom(src => src.AttendanceRecordId))
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.StartTime, tz)))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => src.EndTime.HasValue
                        ? TimeZoneInfo.ConvertTimeFromUtc(src.EndTime.Value, tz)
                        : (DateTime?)null))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));
        }
    }
}
