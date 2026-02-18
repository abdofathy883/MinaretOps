using AutoMapper;
using Core.DTOs.Seo;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class SeoContentProfile : Profile
    {
        public SeoContentProfile()
        {
            CreateMap<SeoContent, SeoContentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords))
                .ForMember(dest => dest.OgTitle, opt => opt.MapFrom(src => src.OgTitle))
                .ForMember(dest => dest.OgDescription, opt => opt.MapFrom(src => src.OgDescription))
                .ForMember(dest => dest.OgImage, opt => opt.MapFrom(src => src.OgImage))
                .ForMember(dest => dest.CanonicalUrl, opt => opt.MapFrom(src => src.CanonicalUrl))
                .ForMember(dest => dest.Robots, opt => opt.MapFrom(src => src.Robots))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
