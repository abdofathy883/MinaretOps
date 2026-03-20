using AutoMapper;
using Application.DTOs.ClientServiceDTOs;
using Core.Models;

namespace Application.MappingProfiles
{
    public class ClientServiceProfile: Profile
    {
        public ClientServiceProfile()
        {
            CreateMap<ClientService, ClientServiceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.ServiceCost, opt => opt.MapFrom(src => src.ServiceCost))
                .ForMember(dest => dest.TaskGroups, opt => opt.MapFrom(src => src.TaskGroups))
                .ForMember(dest => dest.ClientServiceCheckpoints, opt => opt.MapFrom(src => src.ClientServiceCheckpoints));
        }
    }
}
