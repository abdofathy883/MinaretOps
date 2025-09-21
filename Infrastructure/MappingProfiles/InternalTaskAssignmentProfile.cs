using AutoMapper;
using Core.DTOs.InternalTasks;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class InternalTaskAssignmentProfile: Profile
    {
        public InternalTaskAssignmentProfile()
        {
            CreateMap<InternalTaskAssignment, InternalTaskAssignmentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InternalTaskId, opt => opt.MapFrom(src => src.InternalTaskId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.IsLeader, opt => opt.MapFrom(src => src.IsLeader));
        }
    }
}
