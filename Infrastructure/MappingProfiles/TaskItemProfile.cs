using AutoMapper;
using Core.DTOs.Tasks;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class TaskItemProfile: Profile
    {
        public TaskItemProfile()
        {
            CreateMap<TaskItem, TaskDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TaskType, opt => opt.MapFrom(src => src.TaskType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ClientServiceId, opt => opt.MapFrom(src => src.ClientServiceId))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Refrence, opt => opt.MapFrom(src => src.Refrence))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
                .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.IsArchived))
                .ForMember(dest => dest.IsCompletedOnDeadline, opt => opt.MapFrom(src => src.IsCompletedOnDeadline))
                .ForMember(dest => dest.EmployeeName, opt =>
                    opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null))
                .ForMember(dest => dest.TaskGroupId, opt => opt.MapFrom(src => src.TaskGroupId))
                .ForMember(dest => dest.ServiceName, opt =>
                    opt.MapFrom(src => src.ClientService.Service.Title))
                .ForMember(dest => dest.ClientName, opt =>
                    opt.MapFrom(src => src.ClientService.Client.Name))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientService.Client.Id))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ClientService.ServiceId))
                .ForMember(dest => dest.TaskHistory, opt => opt.MapFrom(src => src.TaskHistory))
                .ForMember(dest => dest.TaskResources, opt => opt.MapFrom(src => src.CompletionResources))
                .ForMember(dest => dest.CompletionNotes, opt => opt.MapFrom(src => src.CompletionNotes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
