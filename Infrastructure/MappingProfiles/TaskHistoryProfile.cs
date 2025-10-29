using AutoMapper;
using Core.DTOs.Tasks;
using Core.Models;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class TaskHistoryProfile : Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;
        public TaskHistoryProfile()
        {
            CreateMap<TaskItemHistory, TaskHistoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TaskItemId, opt => opt.MapFrom(src => src.TaskItemId))
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.PropertyName))
                .ForMember(dest => dest.OldValue, opt => opt.MapFrom(src => src.OldValue))
                .ForMember(dest => dest.NewValue, opt => opt.MapFrom(src => src.NewValue))
                .ForMember(dest => dest.UpdatedAt, 
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.UpdatedAt, tz)))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.UpdatedById))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => $"{src.UpdatedBy.FirstName} {src.UpdatedBy.LastName}"));
        }
    }
}
