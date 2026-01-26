using AutoMapper;
using Core.DTOs.Leads;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class LeadProfile : Profile
    {
        public LeadProfile()
        {
            CreateMap<SalesLead, SalesLead>();
            CreateMap<CreateLeadDTO, SalesLead>();
                //.ForMember(dest => dest.ServicesInterestedIn, opt => opt.MapFrom(src => src.ServicesInterestedIn.Select(id => new LeadServices { ServiceId = id }))); // Simple mapping

            CreateMap<SalesLead, LeadDTO>()
                .ForMember(dest => dest.SalesRepName, opt => opt.MapFrom(src => src.SalesRep != null ? src.SalesRep.UserName : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.UserName : null))
                .ForMember(dest => dest.ServicesInterestedIn, opt => opt.MapFrom(src => src.ServicesInterestedIn));

            CreateMap<LeadServices, LeadServicesDTO>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead.BusinessName));
        }
    }
}
