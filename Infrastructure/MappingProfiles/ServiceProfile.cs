using AutoMapper;
using Core.DTOs.Services;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class ServiceProfile: Profile
    {
        public ServiceProfile()
        {
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
        }
    }
}
