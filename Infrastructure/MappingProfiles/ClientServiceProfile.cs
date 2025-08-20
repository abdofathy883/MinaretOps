using AutoMapper;
using Core.DTOs.ClientServiceDTOs;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
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
                //.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                //.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.TaskGroups, opt => opt.MapFrom(src => src.TaskGroups));
                //.ForMember(dest => dest.TotalTasks, opt => opt.MapFrom(src => src.GetAllTasks().Count))
                //.ForMember(dest => dest.CompletedTasks, opt => opt.MapFrom(src =>
                //    src.GetAllTasks().Count(t => t.Status == CustomTaskStatus.Completed)));
        }
    }
}
