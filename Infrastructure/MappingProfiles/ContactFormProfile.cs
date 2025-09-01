using AutoMapper;
using Core.DTOs.ContactFormDTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class ContactFormProfile: Profile
    {
        public ContactFormProfile()
        {
            CreateMap<ContactFormEntryDTO, ContactFormEntry>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DesiredService, opt => opt.MapFrom(src => src.DesiredService))
                .ForMember(dest => dest.ProjectBrief, opt => opt.MapFrom(src => src.ProjectBrief))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp));
        }
    }
}
