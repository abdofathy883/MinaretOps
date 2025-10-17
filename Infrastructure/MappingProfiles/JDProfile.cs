using AutoMapper;
using Core.DTOs.JDs;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class JDProfile : Profile
    {
        public JDProfile()
        {
            CreateMap<JobDescription, JDDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.JobResponsibilities, opt => opt.MapFrom(src => src.JobResponsibilities));

            CreateMap<JobResponsibility, JRDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(opt => opt.Id))
                .ForMember(dest => dest.JobDescriptionId, opt => opt.MapFrom(opt => opt.JobDescriptionId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(opt => opt.Text));
        }
    }
}
