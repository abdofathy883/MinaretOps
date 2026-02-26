using AutoMapper;
using Core.DTOs.Leads;
using Core.Models;
using Infrastructure.Helpers;

namespace Infrastructure.MappingProfiles
{
    public class LeadHistoryProfile : Profile
    {
        private readonly TimeZoneInfo tz = TimeZoneHelper.EgyptTimeZone;

        public LeadHistoryProfile()
        {
            CreateMap<SalesLeadHistory, LeadHistoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SalesLeadId, opt => opt.MapFrom(src => src.SalesLeadId))
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.PropertyName))
                .ForMember(dest => dest.OldValue, opt => opt.MapFrom(src => src.OldValue))
                .ForMember(dest => dest.NewValue, opt => opt.MapFrom(src => src.NewValue))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.UpdatedAt, tz)))
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.UpdatedById))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByName));
        }
    }
}
