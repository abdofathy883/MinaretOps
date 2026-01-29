using AutoMapper;
using Core.DTOs.Leads;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class LeadProfile : Profile
    {
        public LeadProfile()
        {
            CreateMap<SalesLead, LeadDTO>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.BusinessName))
                .ForMember(dest => dest.WhatsAppNumber, opt => opt.MapFrom(src => src.WhatsAppNumber))
                .ForMember(dest => dest.ContactAttempts, opt => opt.MapFrom(src => src.ContactAttempts))
                .ForMember(dest => dest.ContactStatus, opt => opt.MapFrom(src => src.ContactStatus))
                .ForMember(dest => dest.LeadSource, opt => opt.MapFrom(src => src.LeadSource))
                .ForMember(dest => dest.DecisionMakerReached, opt => opt.MapFrom(src => src.DecisionMakerReached))
                .ForMember(dest => dest.Interested, opt => opt.MapFrom(src => src.Interested))
                .ForMember(dest => dest.InterestLevel, opt => opt.MapFrom(src => src.InterestLevel))
                .ForMember(dest => dest.MeetingAgreed, opt => opt.MapFrom(src => src.MeetingAgreed))
                .ForMember(dest => dest.MeetingDate, opt => opt.MapFrom(src => src.MeetingDate))
                .ForMember(dest => dest.MeetingAttend, opt => opt.MapFrom(src => src.MeetingAttend))
                .ForMember(dest => dest.QuotationSent, opt => opt.MapFrom(src => src.QuotationSent))
                .ForMember(dest => dest.FollowUpReason, opt => opt.MapFrom(src => src.FollowUpReason))
                .ForMember(dest => dest.FollowUpTime, opt => opt.MapFrom(src => src.FollowUpTime))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.SalesRepId, opt => opt.MapFrom(src => src.SalesRepId))
                .ForMember(dest => dest.SalesRepName, opt => opt.MapFrom(src => $"{src.SalesRep.FirstName} {src.SalesRep.LastName}"))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}"))
                .ForMember(dest => dest.ServicesInterestedIn, opt => opt.MapFrom(src => src.ServicesInterestedIn));

            CreateMap<LeadServices, LeadServicesDTO>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead.BusinessName));
        }
    }
}
