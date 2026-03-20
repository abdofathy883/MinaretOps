using AutoMapper;
using Application.DTOs.Tasks.CommentDTOs;
using Core.Models;
using Application.Helpers;

namespace Application.MappingProfiles
{
    public class TaskCommentProfile : Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;
        public TaskCommentProfile()
        {
            CreateMap<TaskComment, TaskCommentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null)) // Handle null employee
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, tz)));
        }
    }
}
