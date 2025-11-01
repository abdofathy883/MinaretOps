using AutoMapper;
using Core.DTOs.Tasks.TaskResourcesDTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class TaskResourcesProfile : Profile
    {
        public TaskResourcesProfile()
        {
            CreateMap<TaskCompletionResources, TaskResourcesDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.URL));
        }
    }
}
