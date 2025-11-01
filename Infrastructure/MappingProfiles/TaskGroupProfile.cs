using AutoMapper;
using Core.DTOs.Tasks.TaskGroupDTOs;
using Core.Models;
using Infrastructure.Helpers;

namespace Infrastructure.MappingProfiles
{
    public class TaskGroupProfile: Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;
        public TaskGroupProfile()
        {
            CreateMap<TaskGroup, TaskGroupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Month))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.MonthLabel, opt => opt.MapFrom(src => src.MonthLabel))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                .ForMember(dest => dest.CreatedAt, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, tz)))
                .ForMember(dest => dest.UpdatedAt, 
                opt => opt.MapFrom(src => src.UpdatedAt.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(src.UpdatedAt.Value, tz)
                : (DateTime?)null));
        }
    }
}
