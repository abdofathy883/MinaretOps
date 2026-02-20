using AutoMapper;
using Core.DTOs.Leads;
using Core.DTOs.Leads.Notes;
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
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.ContactStatus, opt => opt.MapFrom(src => src.ContactStatus))
                .ForMember(dest => dest.CurrentLeadStatus, opt => opt.MapFrom(src => src.CurrentLeadStatus))
                .ForMember(dest => dest.LeadSource, opt => opt.MapFrom(src => src.LeadSource))
                .ForMember(dest => dest.FreelancePlatform, opt => opt.MapFrom(src => src.FreelancePlatform))
                .ForMember(dest => dest.InterestLevel, opt => opt.MapFrom(src => src.InterestLevel))
                .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Budget))
                .ForMember(dest => dest.Timeline, opt => opt.MapFrom(src => src.Timeline))
                .ForMember(dest => dest.Responsibility, opt => opt.MapFrom(src => src.Responsibility))
                .ForMember(dest => dest.NeedsExpectation, opt => opt.MapFrom(src => src.NeedsExpectation))
                .ForMember(dest => dest.MeetingDate, opt => opt.MapFrom(src => src.MeetingDate))
                .ForMember(dest => dest.QuotationSent, opt => opt.MapFrom(src => src.QuotationSent))
                .ForMember(dest => dest.FollowUpTime, opt => opt.MapFrom(src => src.FollowUpTime))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.SalesRepId, opt => opt.MapFrom(src => src.SalesRepId))
                .ForMember(dest => dest.SalesRepName, opt => opt.MapFrom(src => src.SalesRep != null ?
                $"{src.SalesRep.FirstName} {src.SalesRep.LastName}" : null))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ?
                $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}" : null))
                .ForMember(dest => dest.ServicesInterestedIn, opt => opt.MapFrom(src => src.ServicesInterestedIn));

            CreateMap<LeadServices, LeadServicesDTO>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead.BusinessName));

            CreateMap<LeadNote, LeadNoteDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.LeadId, opt => opt.MapFrom(src => src.LeadId))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        }
    }
}
