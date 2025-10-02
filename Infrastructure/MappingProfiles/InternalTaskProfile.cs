using AutoMapper;
using Core.DTOs.InternalTasks;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class InternalTaskProfile: Profile
    {
        public InternalTaskProfile()
        {
            CreateMap<InternalTask, InternalTaskDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TaskType, opt => opt.MapFrom(src => src.TaskType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments))
                .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.IsArchived));
        }
    }
}
